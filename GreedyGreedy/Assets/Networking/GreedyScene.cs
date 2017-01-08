using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace GreedyScene {
    public enum ID {//This part need to sync with server
        StartMenu,
        CharacterSelection,
        CharacterCreation,
        Village,
        RootOfEvil,
        Developing,
        Loading
    }
    public static class Maps {
        public static List<ID> Arena = new List<ID> { ID.RootOfEvil };
    }

    /// <summary>
    /// Clients only
    /// </summary>
    [System.Serializable]
    public static class Scene {
        public static AsyncOperation async;       

        public static void Load(ID ID) {
            GameManager.instance.StartCoroutine(LoadOnceLoaded((ID)ID));
        }
        public static void Load(int ID) {
            GameManager.instance.StartCoroutine(LoadOnceLoaded((ID)ID));
        }             

        public static void LoadWithAction(ID ID,Action call) {
            GameManager.instance.StartCoroutine(LoadThenExecute((ID)ID, call));
        }
        public static void LoadWithAction(int ID, Action call) {
            GameManager.instance.StartCoroutine(LoadThenExecute((ID)ID, call));
        }

        public static void LoadWithManualActive(ID ID) {
            GameManager.instance.StartCoroutine(LoadWithSync((ID)ID));
        }
        public static void LoadWithManualActive(int ID) {
            GameManager.instance.StartCoroutine(LoadWithSync((ID)ID));
        }

        public static void LoadWithDelay(ID ID, float time) {
            GameManager.instance.StartCoroutine(LoadWithWait(ID, time));
        }
        public static void LoadWithDelay(int ID, float time) {
            GameManager.instance.StartCoroutine(LoadWithWait((ID)ID, time));
        }

        public static void ActiveScene(float time = 0f,Action call = null) {
            GameManager.instance.StartCoroutine(ActiveSceneUntilLoaded(time,call));
        }

        private static IEnumerator ActiveSceneUntilLoaded(float time,Action call = null) {
            while (async.progress < 0.9f) {
                yield return null;
            }
            yield return new WaitForSeconds(time);            
            async.allowSceneActivation = true;
            if (call != null) {
                while (!async.isDone) {
                    yield return null;                    
                }
                call();
            }
        }
             

        public static IEnumerator LoadWithSync(ID ID) {//Require ActiveScene to active the scene after loaded
            yield return LoadLoadingScreen();
            async = Application.LoadLevelAsync((int)ID);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f) {                                
                yield return null;
            }
            while (!async.isDone)
                yield return null;
        }

        public static IEnumerator LoadWithSyncAndExecute(ID ID,Action call) {//Require ActiveScene to active the scene after loaded
            yield return LoadLoadingScreen();
            async = Application.LoadLevelAsync((int)ID);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f) {
                yield return null;
            }
            while (!async.isDone)
                yield return null;
            call();
        }

        public static IEnumerator LoadOnceLoaded(ID ID) {
            yield return LoadLoadingScreen();            
            async = Application.LoadLevelAsync((int)ID);
            async.allowSceneActivation = false;
            while (async.progress<0.9f) {                                
                yield return null;                
            }
            ActiveScene();
        }

        public static IEnumerator LoadWithWait(ID ID, float time) {//most likely will be deleted
            yield return new WaitForSeconds(time);
            Application.LoadLevel((int)ID);
        }

        public static IEnumerator LoadThenExecute(ID ID, Action Call) {
            yield return LoadLoadingScreen();
            async = Application.LoadLevelAsync((int)ID);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f) {
                yield return null;
            }
            ActiveScene();
            while (!async.isDone)
                yield return null;
            Call();
        }
        public static IEnumerator LoadThenExecute<T>(ID ID,Action<T> Call,T para) {
            yield return LoadLoadingScreen();
            async = Application.LoadLevelAsync((int)ID);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f) {                
                yield return null;
            }
            ActiveScene();
            while (!async.isDone)
                yield return null;
            Call(para);
        }
        public static IEnumerator LoadThenExecute<T1,T2>(ID ID, Action<T1,T2> Call, T1 p1,T2 p2) {
            yield return LoadLoadingScreen();
            async = Application.LoadLevelAsync((int)ID);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f) {                
                yield return null;
            }
            ActiveScene();
            while (!async.isDone)
                yield return null;
            Call(p1,p2);
        }
        public static IEnumerator LoadThenExecute<T1, T2,T3>(ID ID, Action<T1, T2,T3> Call, T1 p1, T2 p2,T3 p3) {
            yield return LoadLoadingScreen();
            async = Application.LoadLevelAsync((int)ID);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f) {
                yield return null;
            }
            ActiveScene();
            while (!async.isDone)
                yield return null;
            Call(p1, p2,p3);
        }

        private static IEnumerator LoadLoadingScreen() {
            AsyncOperation loadingscreen_async = Application.LoadLevelAsync((int)ID.Loading);
            while (!loadingscreen_async.isDone)
                yield return null;            
        }

        public static string Current_Name {
            get { return Application.loadedLevelName; }
        }

        public static ID Current_ID {
            get { return (ID)Application.loadedLevel; }
        }

        public static int Current_Int {
            get { return Application.loadedLevel; }
        }
    }
}
