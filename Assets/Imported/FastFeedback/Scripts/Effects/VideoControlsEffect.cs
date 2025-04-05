using FastFeedback;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoControlsEffect : FastFeedbackEffect
{
    public bool stopRequested = false;
    public float stopPlaybackAt = 0;

    public bool keepPosition;
    public bool hideOnStop;

    private VideoPlayer videoPlayer;

    private long pauseFrame = 0;

    PlayVideo.StopMode stopMode = PlayVideo.StopMode.ResetToStartAndHideVideo;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        if (Time.time > stopPlaybackAt && stopRequested)
        {
            stopRequested = false;

            if (stopMode == PlayVideo.StopMode.KeepPlace) 
            {
                pauseFrame = videoPlayer.frame;
                videoPlayer.Pause();
            }
            else if (stopMode == PlayVideo.StopMode.ResetToStart) 
            {
                videoPlayer.frame = 0;
                videoPlayer.Stop();
            }
            else if (stopMode == PlayVideo.StopMode.KeepPlaceAndHideVideo)
            {
                pauseFrame = videoPlayer.frame;
                videoPlayer.Pause();
                videoPlayer.enabled = false;
            }
            else if (stopMode == PlayVideo.StopMode.ResetToStartAndHideVideo)
            {
                videoPlayer.frame = 0;
                videoPlayer.Stop();
                videoPlayer.enabled = false;
            }
        }        
    }

    public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
    {
        base.Apply(item, target, origin);

        if (item is PlayVideo)
        {
            PlayVideo playVideoItem = (PlayVideo)item;
            videoPlayer.enabled = true;
            videoPlayer.playbackSpeed = playVideoItem.playbackSpeed;
            videoPlayer.isLooping = playVideoItem.loopVideo;
            videoPlayer.frame = pauseFrame;
            videoPlayer.Play();
            stopMode = playVideoItem.stopMode;

            if (playVideoItem.playConditions == PlayVideo.PlayConditions.UntilFinished)
            {
                stopRequested = true;
                stopPlaybackAt = Time.time + (float)(videoPlayer.length * videoPlayer.playbackSpeed);
            }
            else if (playVideoItem.playConditions == PlayVideo.PlayConditions.ForXSeconds)
            {
                stopRequested = true;
                stopPlaybackAt = Time.time + playVideoItem.playForDuration;
            }
            else if (playVideoItem.playConditions == PlayVideo.PlayConditions.XTimes)
            {
                stopRequested = true;
                stopPlaybackAt = Time.time + (float)(videoPlayer.length * videoPlayer.playbackSpeed * playVideoItem.playXTimes);
            }
        }
        else if (item is StopVideo)
        {
            StopVideo stopVideoItem = (StopVideo)item;

            if (stopVideoItem.stopMode == StopVideo.StopMode.ResetToStart)
            {
                videoPlayer.Stop();
                pauseFrame = 0;
            }
            else if (stopVideoItem.stopMode == StopVideo.StopMode.KeepPlace)
            {
                pauseFrame = videoPlayer.frame;
                videoPlayer.Pause();
            }
            else if (stopVideoItem.stopMode == StopVideo.StopMode.KeepPlaceAndHideVideo)
            {
                pauseFrame = videoPlayer.frame;
                videoPlayer.Pause();
                videoPlayer.enabled = false;
            }
            else if (stopVideoItem.stopMode == StopVideo.StopMode.ResetToStartAndHideVideo)
            {
                videoPlayer.Stop();
                videoPlayer.enabled = false;
                pauseFrame = 0;
            }
        }
    }
}
