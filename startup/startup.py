import pyautogui
import time
import cv2
import numpy

# loc = pyautogui.locateOnScreen("google-chrome.png", confidence=0.9)  # 戻り値 Box(left=509, top=1039, width=30, height=30)
# point = pyautogui.locateCenterOnScreen("google-chrome.png", confidence=0.9)  # 戻り値 Point(x=524, y=1054)

if __name__ == "__main__":
    chrome, account, password = None, None, None
    while True:
        if chrome is None:
            chrome = pyautogui.locateCenterOnScreen("google-chrome.png", confidence=0.9)
            if chrome is not None:
                x, y = chrome
                pyautogui.moveTo(x, y, duration=1)
                pyautogui.click(x, y)
                time.sleep(3)

        if account is None:
            account = pyautogui.locateCenterOnScreen("select-acount.png", confidence=0.9)
            if account is not None:
                x, y = account
                pyautogui.moveTo(x, y, duration=1)
                pyautogui.click(x, y)
                time.sleep(3)

        if password is None:
            password = pyautogui.locateCenterOnScreen("password.png", confidence=0.9)
            if password is not None:
                pyautogui.write("Citizen123")
                pyautogui.press("enter")
                break

        time.sleep(1)

    edge, timeworks, tw_login = None, None, None
    while True:
        if edge is None:
            edge = pyautogui.locateCenterOnScreen("microsoft-edge.png", confidence=0.9)
            if edge is not None:
                x, y = edge
                pyautogui.moveTo(x, y, duration=1)
                pyautogui.click(x, y)
                time.sleep(3)

        if timeworks is None:
            timeworks = pyautogui.locateCenterOnScreen("timeworks.png", confidence=0.9)
            if timeworks is not None:
                x, y = timeworks
                pyautogui.moveTo(x, y, duration=1)
                pyautogui.click(x, y)
                time.sleep(3)

        if tw_login is None:
            tw_login = pyautogui.locateCenterOnScreen("timeworks-login.png", confidence=0.6)
            print(tw_login)
            if tw_login is not None:
                pyautogui.write("211107")
                pyautogui.press("tab")
                pyautogui.write("1")
                pyautogui.press("enter")
                break
        time.sleep(1)


    # print(pyautogui.size())
    # # 座標(500, 600)に移動
    # pyautogui.moveTo(200, 200)
    # # 3秒かけて座標(500, 600)に移動
    # pyautogui.moveTo(1000, 800, duration=3)
    # # 2秒かけて現在のカーソルの位置から上に200移動
    # pyautogui.move(0, -200, duration=2)
    # # 左ボタンを押しながら、2秒かけて座標(400, 400)にマウスをドラッグ
    # pyautogui.dragTo(400, 400, duration=2, button="left")
    # # 右ボタンを押しながら、3秒かけて右に200, 下に200の座標までマウスをドラッグ
    # pyautogui.drag(200, 200, duration=3, button="right")
    # 現在のカーソルの位置で左クリックを1回
    # pyautogui.click()
    # # 座標(500, 500)で右クリックを1回
    # pyautogui.click(500, 500, button="right")
    # # 現在のカーソルの位置で0.5秒の間隔で左クリックを3回
    # pyautogui.click(button="left", clicks=3, interval=0.5)
    # moveTo()でカーソルを移動させてクリック
    # pyautogui.moveTo(800, 500)
    # pyautogui.click()enterIt was created using the Python module pyautogui.
    # # click()でカーソルを移動させてクリック
    # pyautogui.click(800, 800)
    # # 0.5秒の間隔でenterという文字列を入力 (Enterキーは押されない)
    # pyautogui.write("enter", interval=0.5)A
    # # 下記の文章も約0.1秒で入力が終えます
    # pyautogui.write("It was created using the Python module pyautogui.")
    # # ctrlを押した状態でaを入力
    # pyautogui.hotkey("ctrl", "a")
    # # 上の例をkeyDown(), keyUp()で書いた場合
    # pyautogui.keyDown("ctrl")
    # pyautogui.keyDown("a")
    # pyautogui.keyUp("a")
    # pyautogui.keyUp("ctrl")

    # # 処理を実行するたびに1秒待機
    # pyautogui.PAUSE = 1.0
    #
    # # 画面のサイズを取得
    # width, height = pyautogui.size()
    #
    # # スタートボタン付近にカーソルを移動しクリック
    # pyautogui.moveTo(5, height - 5, duration=1)
    # pyautogui.click()
    #
    # # メモ帳のアプリを検索し、エンターで起動
    # pyautogui.write("memo")
    # pyautogui.press("enter")
    #
    # # 文字を入力
    # pyautogui.write("It was created using the Python module pyautogui.")
    #
    # # Ctrl+S で保存
    # pyautogui.hotkey("ctrl", "s")
    #
    # # ファイル名を入力し、エンターキーで保存
    # pyautogui.write("Python.txt", interval=0.25)
    # pyautogui.press("enter")
