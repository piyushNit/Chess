using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class AnalyticsManager : MonoBehaviour {
    private static AnalyticsManager _instance;
    public static AnalyticsManager instance {
        get {
            if (_instance == null) _instance = GameObject.FindObjectOfType<AnalyticsManager>();
            return _instance;
        }
    }

    public void MainMenuOpen() {
        DispatchEvent(Events.MAIN_MENU_OPEN);
    }

    public void GameplayOpen() {
        DispatchEvent(Events.GAME_PLAY_OPEN);
    }

    public void UndoBtnPressed() {
        DispatchEvent(Events.UNDO_BTN_PRESSED);
    }

    public void PlayBtnPressed() {
        DispatchEvent(Events.PLAY_BTN_PRESSED);
    }

    public void ShareBtnPressed() {
        DispatchEvent(Events.SHARE_BTN_PRESSED);
    }

    public void FBLikeBtnPressed() {
        DispatchEvent(Events.FB_LIKE_BTN_PRESSED);
    }

    public void ResetBtnPressed() {
        DispatchEvent(Events.RESET_BTN_PRESSED);
    }

    public void QuitTheGame() {
        DispatchEvent(Events.QUIT_THE_GAME);
    }

    private void DispatchEvent(string eventStr, Dictionary<string, object> eventData) {
        Analytics.CustomEvent(eventStr, eventData);
    }

    private void DispatchEvent(string eventStr) {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        Analytics.CustomEvent(eventStr, eventData);
    }
}
