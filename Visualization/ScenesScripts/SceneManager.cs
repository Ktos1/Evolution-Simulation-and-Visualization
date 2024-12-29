using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using ProjectEvolution.Visualization.ScenesStorages;

namespace ProjectEvolution.Visualization.ScenesScripts
{
    /// <summary>
    /// Manages the scenes in the application.
    /// </summary>
    public static class SceneManager
    {
        /// <summary>
        /// The current scene.
        /// </summary>
        private static Node _currentScene;
        /// <summary>
        /// The menu scene.
        /// </summary>
        private static Node _menuScene;
        /// <summary>
        /// The simulation scene.
        /// </summary>
        private static Simulation _simulScene;
        /// <summary>
        /// The visualization scene.
        /// </summary>
        private static Node _visualScene;
        /// <summary>
        /// The root node of the application.
        /// </summary>
        /// <remarks>
        /// Used to add and remove scenes nodes from the scene tree.
        /// </remarks>
        private static Window _root;

        /// <summary>
        /// Initializes the scene manager.
        /// </summary>
        /// <param name="menuScene">
        /// The menu scene.
        /// </param>
        /// <param name="root">
        /// The root node of the application.
        /// </param>
        public static void Initialize(Node menuScene, Window root)
        {
            _menuScene = menuScene;
            _currentScene = menuScene;
            _root = root;
        }

        /// <summary>
        /// Changes the current scene to a new scene.
        /// </summary>
        /// <param name="newScene">
        /// The type of the new scene.
        /// </param>
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

        /// <summary>
        /// Handles the new simulation event.
        /// </summary>
        private static void OnNewSimulation()
        {
            _visualScene = null;
        }

        /// <summary>
        /// Handles the simulation scene reset event.
        /// </summary>
        private static void OnSimulViewReset()
        {
            _simulScene = null;
            ChangeToScene(SceneType.Simulation);
        }
    }

    /// <summary>
    /// Types of scenes available to change to through the <see cref="SceneManager"/>.
    /// </summary>
    public enum SceneType
    {
        Menu,
        Simulation,
        Visualization
    }
}
