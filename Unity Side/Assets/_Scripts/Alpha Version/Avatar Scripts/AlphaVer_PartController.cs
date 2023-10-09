using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for controlling parts of an object in Unity.
/// </summary>
public class AlphaVer_PartController : MonoBehaviour
{
    /// <summary>
    /// A dictionary of parts, with keys representing the name of each part and values representing their corresponding GameObjects.
    /// </summary>
    protected IDictionary<string, GameObject> Parts;

    /// <summary>
    /// Initializes the dictionary of parts.
    /// </summary>
    protected virtual void InitiatePartsDictionary()
    {
        Parts = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// Moves a part by the specified movement amount and waits for the specified time before rewinding the movement.
    /// </summary>
    /// <param name="key">The key for the part to move.</param>
    /// <param name="movement">The amount of movement for the part.</param>
    /// <param name="timeBeforeRewind">The time to wait before rewinding the movement.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    protected IEnumerator MakeMoveOnKey(string key, Vector3 movement, float timeBeforeRewind)
    {
        Vector3 originalPos = new Vector3(Parts[key].transform.position.x, Parts[key].transform.position.y,
            Parts[key].transform.position.z);

        Parts[key].transform.position += movement;

        if (timeBeforeRewind > 0.0f)
        {
            yield return new WaitForSeconds(timeBeforeRewind);
            Parts[key].transform.position = originalPos;
        }
    }

    protected IEnumerator MakeRotationOnKey(string key, Vector3 movement, float timeBeforeRewind)
    {
        Vector3 originalRot = new Vector3(Parts[key].transform.localEulerAngles.x, Parts[key].transform.localEulerAngles.y,
            Parts[key].transform.localEulerAngles.z);

        Parts[key].transform.localEulerAngles += movement;

        if (timeBeforeRewind > 0.0f)
        {
            yield return new WaitForSeconds(timeBeforeRewind);
            Parts[key].transform.localEulerAngles = originalRot;
        }
    }

    protected IEnumerator MoveRotateRewindSpecificKey(string key, Vector3 movement, Vector3 rotation,
        float timeBeforeRewind)
    {
        // Save original state
        Vector3 originalRot = new Vector3(Parts[key].transform.localEulerAngles.x, Parts[key].transform.localEulerAngles.y,
            Parts[key].transform.localEulerAngles.z);
        Vector3 originalPos = new Vector3(Parts[key].transform.position.x, Parts[key].transform.position.y,
            Parts[key].transform.position.z);

        // apply changes
        Parts[key].transform.localEulerAngles += rotation;
        Parts[key].transform.position += movement;

        // rewind if necessary
        if (timeBeforeRewind > 0.0f)
        {
            yield return new WaitForSeconds(timeBeforeRewind);
            Parts[key].transform.localEulerAngles = originalRot;
            Parts[key].transform.position = originalPos;
        }

    }
}
