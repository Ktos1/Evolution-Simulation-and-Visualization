using Godot;
using ProjectEvolution.Utility.BinarySerialization;

namespace ProjectEvolution.Visualization.ScenesScripts
{
    public static class SceneManager
    {
        private static Node _currentScene;
        private static Node _menuScene;
        private static Simulation _simulScene;
        private static Node _visualScene;
        private static Window _root;
        
        public static void Initialize(Node menuScene, Window root)
        {
            _menuScene = menuScene;
            _currentScene = menuScene;
            _root = root;
        }

        public static void ChangeToScene(SceneType newScene)
        {
            Node newSceneNode = null;
            switch (newScene)
            {
                case SceneType.Menu:
                    newSceneNode = _menuScene;
                    break;
                case SceneType.Simulation:
                    if (_simulScene != null) newSceneNode = _simulScene;
                    else
                    {
                        _simulScene = Scenes.SimulationScene.Instantiate() as Simulation;
                        _simulScene.NewSimulation += OnNewSimulation;
                        _simulScene.ResetToDefault += OnSimulViewReset;
                        newSceneNode = _simulScene;
                    }
                    break;
                case SceneType.Visualization:
                    if (_visualScene != null) newSceneNode = _visualScene;
                    else
                    {
                        BinReader.LoadNewFile();
                        _visualScene = Scenes.VisualizationScene.Instantiate();
                        newSceneNode = _visualScene;
                    }
                    break;
            }
            _root.RemoveChild(_currentScene);
            _root.AddChild(newSceneNode);
            _currentScene = newSceneNode;
        }

        private static void OnNewSimulation()
        {
            _visualScene = null;
        }

        private static void OnSimulViewReset()
        {
            _simulScene = null;
            ChangeToScene(SceneType.Simulation);
        }
    }

    public enum SceneType
    {
        Menu,
        Simulation,
        Visualization
    }
}
