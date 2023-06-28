import cv2
import depthai as dai
import numpy as np
import threading
import time


def create_pipeline(stereo_depth=False):
    pipeline = dai.Pipeline()
    # 解像度については https://docs.luxonis.com/projects/hardware/en/latest/pages/DM9095.html
    # カラーカメラのノード作成
    color_camera = pipeline.create(dai.node.ColorCamera)
    color_camera.setResolution(dai.ColorCameraProperties.SensorResolution.THE_13_MP)
    color_camera.setBoardSocket(dai.CameraBoardSocket.RGB)
    color_camera.setColorOrder(dai.ColorCameraProperties.ColorOrder.RGB)
    # color_camera.setInterleaved(False)

    # カラーカメラ用のxlinkノード作成
    xout_color_camera = pipeline.create(dai.node.XLinkOut)
    xout_color_camera.setStreamName("color_camera")
    color_camera.isp.link(xout_color_camera.input)
    # IMX214はMax Resolusion=THE_13_MP(4208x3120)
    # SensorResolution.THE_13_MPの時　rawの場合 output->(16411200,)だった ispの場合output->(3120,4208,3)だった　videoの場合 output->(2160,3840,3)だった previewの場合はoutput->(300,300,3)だった
    # SensorResolution.THE_1080_Pの時　rawの場合 output->(2592000,)だった ispの場合output->(1080,1920,3)だった　videoの場合 output->(1080,1920,3)だった previewの場合はoutput->(300,300,3)だった

    if stereo_depth:
        # 左モノクロカメラのノード作成
        mono_camera_left = pipeline.create(dai.node.MonoCamera)
        mono_camera_left.setResolution(dai.MonoCameraProperties.SensorResolution.THE_480_P)
        mono_camera_left.setBoardSocket(dai.CameraBoardSocket.LEFT)
        # 左モノクロ用のxlinkノード作成
        xout_mono_camera_left = pipeline.create(dai.node.XLinkOut)
        xout_mono_camera_left.setStreamName("mono_camera_left")
        mono_camera_left.out.link(xout_mono_camera_left.input)

        # 右モノクロカメラのノード作成
        mono_camera_right = pipeline.create(dai.node.MonoCamera)
        mono_camera_right.setResolution(dai.MonoCameraProperties.SensorResolution.THE_480_P)
        mono_camera_right.setBoardSocket(dai.CameraBoardSocket.RIGHT)
        # 右モノクロ用のxlinkノード作成
        xout_mono_camera_right = pipeline.create(dai.node.XLinkOut)
        xout_mono_camera_right.setStreamName("mono_camera_right")
        mono_camera_right.out.link(xout_mono_camera_right.input)

        # ステレオ深度ノードを作成
        stereo_depth = pipeline.create(dai.node.StereoDepth)
        stereo_depth.setDepthAlign(dai.CameraBoardSocket.RGB)
        # stereo.setDefaultProfilePreset(dai.node.StereoDepth.PresetMode.HIGH_DENSITY)
        mono_camera_left.out.link(stereo_depth.left)
        mono_camera_right.out.link(stereo_depth.right)

        # ステレオ深度データ出力用ノードを作成
        xout_depth = pipeline.createXLinkOut()
        xout_depth.setStreamName("depth")
        stereo_depth.depth.link(xout_depth.input)

        # 視差データ出力用ノードを作成
        xout_disp = pipeline.createXLinkOut()
        xout_disp.setStreamName("disparity")
        stereo_depth.disparity.link(xout_disp.input)

    return pipeline


def view_rightMonoCamera(device):
    if device is None:
        return
    right_q = device.getOutputQueue(name="mono_camera_right", maxSize=4, blocking=True)
    while True:
        right_data = right_q.tryGet()
        if right_data is not None:
            right_frame = right_data.getCvFrame()
            cv2.imshow(f"right mono camera {right_frame.shape}", right_frame)


if __name__ == "__main__":

    with dai.Device() as device:
        if len(device.getConnectedCameras()) > 1:
            stereo = True
        pipeline = create_pipeline(stereo)

        # Upload the pipeline to the device
        success = device.startPipeline(pipeline)
        if success:
            # Print MxID, USB speed, and available cameras on the device
            print('MxId:', device.getDeviceInfo().getMxId())
            print('USB speed:', device.getUsbSpeed())
            print('Connected cameras:', device.getConnectedCameras())

            # thread1 = threading.Thread(target=view_rightMonoCamera(device))
            # thread1.start()

            # Output queue, to receive message on the host from the device (you can send the message on the device with XLinkOut)
            cam_q = device.getOutputQueue(name="color_camera", maxSize=4, blocking=True)
            left_q = device.getOutputQueue(name="mono_camera_left", maxSize=4, blocking=True)
            right_q = device.getOutputQueue(name="mono_camera_right", maxSize=4, blocking=True)
            depth_q = device.getOutputQueue(name="depth", maxSize=4, blocking=True)
            disp_q = device.getOutputQueue(name="disparity", maxSize=4, blocking=True)

            while True:
                # Get a message that came from the queue
                cam_data = cam_q.get()  # depthai.ImgFrame object
                left_data = left_q.tryGet()
                right_data = right_q.tryGet()
                depth_data = depth_q.tryGet()  # depthai.ImgFrame object  UINT16 values - depth in depth units (millimeter by default)
                disp_data = disp_q.tryGet()  # depthai.ImgFrame object  UINT8 or UINT16 if Subpixel mode

                if left_data is not None:
                    left_frame = left_data.getCvFrame()
                    cv2.imshow(f"left mono camera {left_frame.shape}", left_frame)
                if right_data is not None:
                    right_frame = right_data.getCvFrame()
                    cv2.imshow(f"right mono camera {right_frame.shape}", right_frame)

                if cam_data is not None:
                    cam_frame = cam_data.getCvFrame()  # <class 'numpy.ndarray'> color cameraのSensorResolution.THE_13_MPの時 shape = (3120, 4208)　・・・outut = isp
                    # カラーカメラ画像を画面に表示する
                    cam_frame = cv2.resize(cam_frame, (640, 480))
                    cv2.imshow(f"color camera {cam_frame.shape}", cam_frame)

                if disp_data is not None:
                    disp_frame = disp_data.getCvFrame()  # <class 'numpy.ndarray'> color cameraのSensorResolution.THE_13_MPの時 shape = (3120, 4208)
                    disp_frame = cv2.resize(disp_frame, (640, 480))
                    # カメラに近いところは白い(大きい値)、カメラから遠いところは黒い(小さい値)。 デフォルトの設定（標準モード）では、視差データは0～95の値をとる。 この値を 0～255 にする
                    disp_frame = np.round(disp_frame * (255 / 95)).astype(np.uint8)
                    disp_frame = cv2.applyColorMap(disp_frame, cv2.COLORMAP_JET)
                    # # 視差データフレームを画面に表示する
                    cv2.imshow(f"disparity {disp_frame.shape}", disp_frame)

                if depth_data is not None:
                    depth_frame = depth_data.getCvFrame()  # numpy配列 SensorResolution.THE_13_MPの時 shape = (3120, 4208)  SensorResolution.THE_1080_Pの時 shape =(1080, 1920)
                    depth_frame = cv2.resize(depth_frame, (640, 480))
                    depth_frame = np.where((depth_frame > 5000) | (depth_frame < 350), np.nan, depth_frame)
                    # print(depth_frame[240, 320]," mm")
                    # depth データフレームを画面に表示する
                    depth_frame = depth_frame * (255 / 5000)
                    depth_frame = -1 * (depth_frame - 255)  # 近い方を高い値にする
                    # depth_frame = np.where(depth_frame == np.nan, 0, depth_frame)
                    depth_frame = np.round(depth_frame).astype(np.uint8)

                    depth_frame = cv2.applyColorMap(depth_frame, cv2.COLORMAP_JET)
                    cv2.imshow(f"depth {depth_frame.shape}", depth_frame)

                if cv2.waitKey(1) == ord('q'):
                    break
        else:
            print("Start pipeline is failed")

    print("end")
