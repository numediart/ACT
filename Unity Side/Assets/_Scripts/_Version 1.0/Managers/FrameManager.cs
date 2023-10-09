using System;

[Serializable]
public class FrameManager
{
    public int FrameNb;
    public int Fps;
    public int MaxFrameNb;
    public int FrameNbToStopIfTransition;
    public int FrameNbToStartIfTransition;
    
    public void Init(Frame[] frames)
    {
        MaxFrameNb = frames.Length;
        CalculateFpsFromFrameTab(frames);
        CalculateFramesToStopAndStartIfTransition();
        FrameNb = 0;
    }

    private void CalculateFramesToStopAndStartIfTransition()
    {
        float transitionDurationRef = MainManager.Instance.TransitionDuration;

        FrameNbToStartIfTransition = (int)Math.Round(Fps * transitionDurationRef * 0.5, MidpointRounding.AwayFromZero);
        FrameNbToStopIfTransition = (MaxFrameNb - FrameNbToStartIfTransition);
    }
    
    /// <summary>
    /// Calculates the frames per second (FPS) based on an array of frames.
    /// </summary>
    /// <param name="frames">The array of frames.</param>
    private void CalculateFpsFromFrameTab(Frame[] frames)
    {
        if (frames.Length > 1)
        {
            double timeOne = frames[0].Timestamp;
            double timeTwo = frames[1].Timestamp;

            Fps = (int) (1 / (timeTwo - timeOne));
        }

        // Fix for OpenFace observed after several CSV imports : the clips are speeded up by 5 FPS (the timestamp >= 1.0d should happen 5 FPS earlier
        //Fps -= 5;
    }

    public void FrameExecuted(bool willTransitionToOtherAction)
    {
        if (willTransitionToOtherAction && FrameNb < FrameNbToStopIfTransition - 1)
        {
            FrameNb++;
        }
        else if (!willTransitionToOtherAction && FrameNb < MaxFrameNb - 1)
        {
            FrameNb++;
        }
        else
        {
            FrameNb = 0;
            MainManager.Instance.EndOfActionEvent();
        }
    }

    public void SkipFramesForTransition()
    {
        FrameNb = FrameNbToStartIfTransition;
    }
}
