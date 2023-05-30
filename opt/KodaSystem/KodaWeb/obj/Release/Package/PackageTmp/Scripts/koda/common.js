"use strict";

//EnterでForm送信しないようにする
$("input").on("keydown", function (e) {
    if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
        return false;
    } else {
        return true;
    }
});

//inputタグでソフトキーボード出さないようにする
$(".use_keyboard").attr("inputmode", "none");
$(".use_keyboard").removeAttr("type");

$("body").on("click", ".toggle_off_icon", function () {
    $(".use_keyboard").removeAttr("inputmode");
    $(".use_keyboard").attr("type", "text");
    $(this).addClass("toggle_on_icon");
    $(this).removeClass("toggle_off_icon");
});

$("body").on("click", ".toggle_on_icon", function () {
    $(".use_keyboard").removeAttr("type");
    $(".use_keyboard").attr("inputmode", "none");
    $(this).addClass("toggle_off_icon");
    $(this).removeClass("toggle_on_icon");
});
//$(".change_inputmode").attr("type", "tel");
//$(".change_inputmode").attr("inputmode", "none");
//$(".change_inputmode").removeAttr("inputmode");
//$(".change_inputmode").removeAttr("type");

// クリック操作時にtextBox等の中身を空にする関数
function onClick_clearInput(clickID, clearID) {
    $(clickID).on("click", function () {
        $(clearID).val("");
        $(clearID).focus();
    });
}

// 機種名＋LotNoの35文字を機種名とLotNoに分けて返す関数
function getProductLotno(code) {
    const lotno10 = code.substr(-10);
    const product = code.substring(0, code.length - lotno10.length).trim()
    return { product: product, lotno10: lotno10 };
}

// utf8のcsvファイルをダウンロードする
function csvDownload(filename, csvText) {
    const bom = new Uint8Array([0xef, 0xbb, 0xbf]);
    let blob = new Blob([bom, csvText], { type: "text/csv" });

    let downloadLink = document.createElement("a");
    downloadLink.download = filename;
    downloadLink.href = URL.createObjectURL(blob);
    downloadLink.click()
}

//ツリーの選択されているノードを取得する
function getNodes(data) {

    let nodes = [];

    //dataは選択されたnodeオブジェクト
    nodes.push(data.text);
    while (true) {
        let parent = $("#tree").treeview("getNode", data.parentId)

        //parent nodeが無い時はJQueryオブジェクトが返ってくる
        if (parent instanceof jQuery) {
            return nodes;
        } else {
            nodes.push(parent.text);
        }
        data = parent;
    }
}

//カメラでQR読み取る為に使用するクラス
class ReadQRcodeCamera {

    #camera_stop = false;
    #canvasElement;
    #canvas;
    #camera_zoom = 1;

    constructor(canvasID) {
        this.#canvasElement = document.getElementById(canvasID);
        this.#canvas = this.#canvasElement.getContext("2d");
    }

    set Zoom(val) {
        this.#camera_zoom = val;
    }

    //カメラをストップさせる関数
    cameraStop() {
        this.#camera_stop = true;
    }

    // カメラ開いてQRcode読み取る関数
    cameraStart() {

        // videoエレメント作成
        const video = document.createElement("video");

        // カメラ設定
        const constraints = {
            audio: false,
            video: {
                // facingMode: "user"   // フロントカメラを利用する
                // facingMode: { exact: "environment" }  // 絶対にリアカメラを利用する場合（無い場合は起動しない）
                facingMode: "environment" // リアカメラを利用する場合
            }
        };

        // カメラ画像の取得は良く使われている"navigator.mediaDevices.getUserMedia"を使う
        navigator.mediaDevices.getUserMedia(constraints)
            .then((stream) => {
                video.srcObject = stream;
                video.play();
                // "requestAnimationFrame"の引数に実行したい関数を渡すだけで、その関数がブラウザにとって最適なタイミングで処理される
                requestAnimationFrame(tick.bind(this));
            })
            .catch(err => alert(err));

        function tick() {
            //フォーカスがあるエレメント取得
            let activeElement = document.activeElement;

            if (video.readyState === video.HAVE_ENOUGH_DATA) {

                //ズーム領域設定してcanvasにその部分を描画
                let zoom_width = video.videoWidth / this.#camera_zoom
                let zoom_height = video.videoHeight / this.#camera_zoom
                let zoom_posX = (video.videoWidth - zoom_width) / this.#camera_zoom
                let zoom_posY = (video.videoHeight - zoom_height) / this.#camera_zoom
                this.#canvas.drawImage(video, zoom_posX, zoom_posY, zoom_width, zoom_height, 0, 0, this.#canvasElement.width, this.#canvasElement.height);

                // canvasから再度画像取得
                let imageData = this.#canvas.getImageData(0, 0, this.#canvasElement.width, this.#canvasElement.height);
                // 画像をjsQRライブラリを使用してQRコード読み取り
                let code = jsQR(imageData.data, imageData.width, imageData.height, {
                    inversionAttempts: "dontInvert",
                });

                // QRコードが読み取れたなら
                if (code) {
                    // 赤枠描く
                    this.#drawLine(code.location.topLeftCorner, code.location.topRightCorner, "red");
                    this.#drawLine(code.location.topRightCorner, code.location.bottomRightCorner, "red");
                    this.#drawLine(code.location.bottomRightCorner, code.location.bottomLeftCorner, "red");
                    this.#drawLine(code.location.bottomLeftCorner, code.location.topLeftCorner, "red");
                    // フォーカスがあるエレメントに読み取りデータ書き込み
                    activeElement.value = code.data;
                    //inputイベントが発動しない為、←キーを押すkeydownイベント発生させる
                    activeElement.dispatchEvent(new KeyboardEvent("keydown", { keyCode: 37 }));
                }
            }

            //カメラストップの合図があったら
            if (this.#camera_stop) {
                //ストップ＋リソース解放
                video.srcObject.getTracks().forEach(track => track.stop())
                video.srcObject = null;
                return;
            }
            //この関数を再度実行（ループさせる）
            requestAnimationFrame(tick.bind(this));
        }
    }

    // 線描く関数
    #drawLine(begin, end, color) {
        this.#canvas.beginPath();
        this.#canvas.moveTo(begin.x, begin.y);
        this.#canvas.lineTo(end.x, end.y);
        this.#canvas.lineWidth = 4;
        this.#canvas.strokeStyle = color;
        this.#canvas.stroke();
    }

}

//未完成
class Conversion {

    static nowCcomposition = false;
    #myElement;

    constructor() {
    }

    toHankaku(str) {
        if (nowCcomposition === true) {
            return str;
        }
        else {
            console.log(str);
            const han = str.replace(/[０-９Ａ-Ｚａ-ｚ]/g, function (es) {
                return String.fromCharCode(es.charCodeAt(0) - 65248);
            });
            console.log(han);
            return han;
        }
    }


}