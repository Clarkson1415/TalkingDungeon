using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FastFeedback;
using UnityEditor.SceneManagement;
using UnityEngine.Events;

[CustomEditor(typeof(GameFeedback))]
public class GameFeedbackEditor : Editor
{
    //public List<List<EventFeedback>> feedbackGroups = new List<List<EventFeedback>>();

    public int currentFeedbackGroup = 0;

    public UnityEvent temp;

    public List<GameFeedback> feedback = new List<GameFeedback>();

    public static void DrawAll (GameFeedback feedback) 
    {
        List<FeedbackItem> toDestroy = new List<FeedbackItem>();

        EditorGUILayout.Space(10);

        if (feedback.feedbackItems.Count == 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUIStyle s = new GUIStyle(GUI.skin.label);
            s.wordWrap = true;
            s.alignment = TextAnchor.UpperCenter;
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("No feedback has been added to this group yet!\n\nPress the 'Add Feedback' button below to add appropriate feedback to the event.", s);
            EditorGUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }

        foreach (FeedbackItem item in feedback.feedbackItems)
        {

            if (item.hasError) GUI.color = new Color(1.0f, 0.5f, 0.5f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            // Draw the header, with the appropriate header icon and description for this item.
            EditorGUILayout.BeginHorizontal(GUILayout.Height(24));

            GUILayout.Label(item.GetIcon(), GUILayout.Width(24), GUILayout.Height(24));
            item.isEnabled = GUILayout.Toggle(item.isEnabled, "", GUILayout.Width(14), GUILayout.Height(24));
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(item.GetDescription(), style, GUILayout.Height(24));

            // Mark this feedback item for deletion if the delete button is pressed.
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.padding = new RectOffset(2, 2, 2, 2);
            btnStyle.margin = new RectOffset(0, 0, 5, 0);
            if (GUILayout.Button((Texture2D)EditorGUIUtility.Load(@"d_winbtn_win_close@2x"), btnStyle, GUILayout.Height(20), GUILayout.Width(20)))
                toDestroy.Add(item);

            // Draw the custom elements for this feedback item.
            EditorGUILayout.EndHorizontal();


            //if (item.isExpanded)
            //EditorGUILayout.Space(5);

            Rect r = EditorGUILayout.BeginVertical();
            Rect rNew = new Rect(r.xMin - 23, r.yMin - 25, 20, 20);


            if (item.isExpanded)
            {
                EditorGUILayout.Space(5);
                Texture2D tex = (Texture2D)EditorGUIUtility.Load(@"icon dropdown@2x");
                GUIStyle btn = new GUIStyle(GUI.skin.button);
                btn.normal.background = tex;

                if (GUI.Button(rNew, tex, btn))
                {
                    item.isExpanded = !item.isExpanded;
                }

                EditorGUI.BeginChangeCheck();
                item.DrawUI(feedback);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(feedback);
                }
                EditorGUILayout.Space(5);
            }
            else
            {
                Texture2D tex = (Texture2D)EditorGUIUtility.Load(@"PlayButton@2x");
                GUIStyle btn = new GUIStyle(GUI.skin.button);
                btn.normal.background = tex;

                if (GUI.Button(rNew, tex, btn))
                {
                    item.isExpanded = !item.isExpanded;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        // Remove any deleted feedback items in this group.
        foreach (FeedbackItem item in toDestroy)
            feedback.feedbackItems.Remove(item);

        // Create the 'Add Feedback' button.
        string addFeedbackLabel = "Add Feedback";
        if (feedback.feedbackItems.Count > 0) addFeedbackLabel = "Add More Feedback";
        if (GUILayout.Button(addFeedbackLabel))
        {
            GenericMenu menu = new GenericMenu();

            List<FeedbackItem> items = feedback.feedbackItems;

            menu.AddItem(new GUIContent("Game Object - Create"), false,
                () => { AddItem(feedback, new CreateGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Destroy"), false,
                () => { AddItem(feedback, new DestroyGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Enable"), false,
                () => { AddItem(feedback, new EnableGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Disable"), false,
                () => { AddItem(feedback, new DisableGameObject()); });

            menu.AddItem(new GUIContent("Renderer - Pulse Size"), false,
                () => { AddItem(feedback, new PulseSize()); });

            menu.AddItem(new GUIContent("Renderer - Flash Color"), false,
                () => { AddItem(feedback, new FlashColor()); });

            menu.AddItem(new GUIContent("Sound - Play"), false,
                () => { AddItem(feedback, new PlaySound()); });

            menu.AddItem(new GUIContent("Particle System - Emit"), false,
                () => { AddItem(feedback, new EmitParticles()); });

            menu.AddItem(new GUIContent("Animation - Play"), false,
                () => { AddItem(feedback, new PlayAnimation()); });

            menu.AddItem(new GUIContent("Animator - Set Trigger"), false,
                () => { AddItem(feedback, new SetAnimatorTrigger()); });

            menu.AddItem(new GUIContent("Animator - Set Variable Value"), false,
                () => { AddItem(feedback, new SetAnimatorVariable()); });

            menu.AddItem(new GUIContent("Camera - Flash"), false,
                () => { AddItem(feedback, new ScreenFlash()); });

            menu.AddItem(new GUIContent("Camera - Shake"), false,
                () => { AddItem(feedback, new CameraShake()); });

            menu.AddItem(new GUIContent("Camera - Add Shader Effect"), false,
                () => { AddItem(feedback, new ApplyCameraShader()); });

            menu.AddItem(new GUIContent("Camera - Remove Shader Effect"), false,
                () => { AddItem(feedback, new RemoveCameraShader()); });

            menu.AddItem(new GUIContent("Haptics - Vibrate Controller"), false,
                () => { AddItem(feedback, new VibrateController()); });

            menu.AddItem(new GUIContent("Haptics - Vibrate Mobile"), false,
                () => { AddItem(feedback, new VibrateMobile()); });

            menu.AddItem(new GUIContent("Video - Play Video"), false,
                () => { AddItem(feedback, new PlayVideo()); });

            menu.AddItem(new GUIContent("Video - Stop Video"), false,
                () => { AddItem(feedback, new StopVideo()); });

            menu.AddItem(new GUIContent("Post Processing - Flash Profile"), false,
                () => { AddItem(feedback, new PPFlashProfile()); });

            menu.AddItem(new GUIContent("Time - Timescale"), false,
                () => { AddItem(feedback, new ScaleTime()); });

            menu.ShowAsContext();
        }
    }

    public override void OnInspectorGUI()
    {
        GameFeedback feedback = (GameFeedback)target;
        DrawAll(feedback);
        return;
        /*
        // Otherwise, display the popup allowing the user to select a feedback group to view

        List<FeedbackItem> toDestroy = new List<FeedbackItem>();

        EditorGUILayout.Space(10);

        if (feedback.feedbackItems.Count == 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUIStyle s = new GUIStyle(GUI.skin.label);
            s.wordWrap = true;
            s.alignment = TextAnchor.UpperCenter;
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("No feedback has been added to this group yet!\n\nPress the 'Add Feedback' button below to add appropriate feedback to the event.", s);
            EditorGUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }

        foreach (FeedbackItem item in feedback.feedbackItems)
        {

            if (item.hasError) GUI.color = new Color(1.0f, 0.5f, 0.5f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            // Draw the header, with the appropriate header icon and description for this item.
            EditorGUILayout.BeginHorizontal(GUILayout.Height(24));

            GUILayout.Label(item.GetIcon(), GUILayout.Width(24), GUILayout.Height(24));
            item.isEnabled = GUILayout.Toggle(item.isEnabled, "", GUILayout.Width(14), GUILayout.Height(24));
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(item.GetDescription(), style, GUILayout.Height(24));

            // Mark this feedback item for deletion if the delete button is pressed.
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.padding = new RectOffset(2, 2, 2, 2);
            btnStyle.margin = new RectOffset(0, 0, 5, 0);
            if (GUILayout.Button((Texture2D)EditorGUIUtility.Load(@"d_winbtn_win_close@2x"), btnStyle, GUILayout.Height(20), GUILayout.Width(20)))
                toDestroy.Add(item);

            // Draw the custom elements for this feedback item.
            EditorGUILayout.EndHorizontal();


            //if (item.isExpanded)
            //EditorGUILayout.Space(5);

            Rect r = EditorGUILayout.BeginVertical();
            Rect rNew = new Rect(r.xMin - 23, r.yMin - 25, 20,  20);


            if (item.isExpanded)
            {
                EditorGUILayout.Space(5);
                Texture2D tex = (Texture2D)EditorGUIUtility.Load(@"icon dropdown@2x");
                GUIStyle btn = new GUIStyle(GUI.skin.button);
                btn.normal.background = tex;

                if (GUI.Button(rNew, tex, btn))
                {
                    item.isExpanded = !item.isExpanded;
                }

                EditorGUI.BeginChangeCheck();
                item.DrawUI(feedback);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(feedback);
                }
                EditorGUILayout.Space(5);
            }
            else
            {
                Texture2D tex = (Texture2D)EditorGUIUtility.Load(@"PlayButton@2x");
                GUIStyle btn = new GUIStyle(GUI.skin.button);
                btn.normal.background = tex;

                if (GUI.Button(rNew, tex, btn))
                {
                    item.isExpanded = !item.isExpanded;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        // Remove any deleted feedback items in this group.
        foreach (FeedbackItem item in toDestroy)
            feedback.feedbackItems.Remove(item);

        // Create the 'Add Feedback' button.
        string addFeedbackLabel = "Add Feedback";
        if (feedback.feedbackItems.Count > 0) addFeedbackLabel = "Add More Feedback";
        if (GUILayout.Button(addFeedbackLabel))
        {
            GenericMenu menu = new GenericMenu();

            List<FeedbackItem> items = feedback.feedbackItems;

            menu.AddItem(new GUIContent("Game Object - Create"), false,
                () => { AddItem(feedback, new CreateGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Destroy"), false,
                () => { AddItem(feedback, new DestroyGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Enable"), false,
                () => { AddItem(feedback, new EnableGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Disable"), false,
                () => { AddItem(feedback, new DisableGameObject()); });

            menu.AddItem(new GUIContent("Renderer - Pulse Size"), false,
                () => { AddItem(feedback, new PulseSize()); });

            menu.AddItem(new GUIContent("Renderer - Flash Color"), false,
                () => { AddItem(feedback, new FlashColor()); });

            menu.AddItem(new GUIContent("Sound - Play"), false,
                () => { AddItem(feedback, new PlaySound()); });

            menu.AddItem(new GUIContent("Particle System - Emit"), false,
                () => { AddItem(feedback, new EmitParticles()); });

            menu.AddItem(new GUIContent("Animation - Play"), false,
                () => { AddItem(feedback, new PlayAnimation()); });

            menu.AddItem(new GUIContent("Animator - Set Trigger"), false,
                () => { AddItem(feedback, new SetAnimatorTrigger()); });

            menu.AddItem(new GUIContent("Animator - Set Variable Value"), false,
                () => { AddItem(feedback, new SetAnimatorVariable()); });

            menu.AddItem(new GUIContent("Camera - Flash"), false,
                () => { AddItem(feedback, new ScreenFlash()); });

            menu.AddItem(new GUIContent("Camera - Shake"), false,
                () => { AddItem(feedback, new CameraShake()); });

            menu.AddItem(new GUIContent("Camera - Add Shader Effect"), false,
                () => { AddItem(feedback, new ApplyCameraShader()); });

            menu.AddItem(new GUIContent("Camera - Remove Shader Effect"), false,
                () => { AddItem(feedback, new RemoveCameraShader()); });

            menu.AddItem(new GUIContent("Outline - Add Object Outline"), false,
                () => { AddItem(feedback, new EnableOutlineWithColor()); });

            menu.AddItem(new GUIContent("Outline - Remove Object Outlines"), false,
                () => { AddItem(feedback, new DisableOutline()); });

            menu.AddItem(new GUIContent("Haptics - Vibrate Controller"), false,
                () => { AddItem(feedback, new VibrateController()); });

            menu.AddItem(new GUIContent("Haptics - Vibrate Mobile"), false,
                () => { AddItem(feedback, new VibrateMobile()); });

            menu.AddItem(new GUIContent("Video - Play Video"), false,
                () => { AddItem(feedback, new PlayVideo()); });

            menu.AddItem(new GUIContent("Video - Stop Video"), false,
                () => { AddItem(feedback, new StopVideo()); });

            menu.AddItem(new GUIContent("Post Processing - Flash Profile"), false,
                () => { AddItem(feedback, new PPFlashProfile()); });

            menu.ShowAsContext();
        }
        */
    }

    protected static void AddItem(GameFeedback feedback, FeedbackItem item)
    {
        feedback.feedbackItems.Add(item);
        EditorUtility.SetDirty(feedback);
    }
}
