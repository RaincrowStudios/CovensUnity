using System;
using System.IO;
using Google.Maps;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// An interface between benchmarking scenarios and the benchmark runner.
/// </summary>
/// <remarks>
/// Each benchmarking scenario scene should include a MonoBehaviour that implements this, and run
/// it as a component inside a GameObject called "ScenarioMonitor".
/// </remarks>
public abstract class BenchmarkingScenario : MonoBehaviour {
  /// <summary>
  /// Returns true when the scenario has finished running.
  /// </summary>
  public abstract bool IsDone();

  /// <summary>
  /// Returns the results found by running the scenario.
  /// </summary>
  public abstract string GetResults();

  /// <summary>
  /// A collection of MapsServices running in the benchmarking scenario. These are stored so their
  /// disk caches can be purged during a scenario teardown.
  /// </summary>
  private MapsService[] MapsServices;

  /// <summary>
  /// Performs teardown of the benchmarking scenario when the containing scene is unloaded. A
  /// correct teardown is required to maintain a sterile working environment for the next test to
  /// be run.
  /// </summary>
  /// <param name="scene">The Unity scene being unloaded.</param>
  protected virtual void Teardown(Scene scene) {
    // Purge the disk caches of all MapsService components in this scene.
    foreach (MapsService mapsService in MapsServices) {
      string cachePath = mapsService.CacheOptions.BasePath;
      if (String.IsNullOrEmpty(cachePath)) {
        cachePath = Application.temporaryCachePath;
      }

      Directory.Delete(cachePath, true);
    }

    SceneManager.sceneUnloaded -= Teardown;
  }

  void Awake() {
    // This is done here instead of in Teardown because FindObjectsOfType does not work during a
    // scene unload.
    MapsServices = Resources.FindObjectsOfTypeAll<MapsService>();

    SceneManager.sceneUnloaded += Teardown;
  }
}
