using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FastFeedback;
using UnityEditor.SceneManagement;
using UnityEngine.Events;

[CustomEditor(typeof(EventFeedback))]
public class EventFeedbackEditor : Editor
{
    public List<List<EventFeedback>> feedbackGroups = new List<List<EventFeedback>>();

    public int currentFeedbackGroup = 0;

    private bool isEditingName = false;

    private string tempName = "";

    public UnityEvent temp;

    public override void OnInspectorGUI()
    {
        EventFeedback feedback = (EventFeedback)target;

        

        // There must always be at least one feedback group.
        if (feedback.feedbackGroups.Count == 0)
        {
            feedback.feedbackGroups.Add(new EventFeedback.EventFeedbackGroup());
        }

        EditorGUILayout.Space(10);

        if (!feedback.isSetup)
        {
            EditorGUILayout.HelpBox("Feedback is activated in groups. Each Feedback Group should correspond to an in-game event, such as a a character taking damage or an enemy dying. Each group can have multiple types of feedback added.\n\nEnter a name for your first feedback group, and then press the confirm button to begin adding feedback.", MessageType.Info);
            EditorGUILayout.Space(10);
        }

        // If the user is naming the current group, display that window.
        if (isEditingName || !feedback.isSetup)
        {
            tempName = EditorGUILayout.TextArea(tempName);
            if (tempName.Length == 0) 
                GUI.enabled = false;
            if (GUILayout.Button("Confirm Feedback Group Name"))
            {
                feedback.feedbackGroups[currentFeedbackGroup].feedbackGroupName = tempName;
                isEditingName = false;
                feedback.isSetup = true;
                if (PrefabUtility.GetPrefabInstanceStatus(feedback.gameObject) == PrefabInstanceStatus.NotAPrefab)
                {
                    EditorUtility.SetDirty(feedback);
                }

                else
                {
                    if (!Application.isPlaying)
                    {
                        EditorUtility.SetDirty(feedback);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
            }
            GUI.enabled = true;
        }

        // Otherwise, display the popup allowing the user to select a feedback group to view.
        else
        {
            string[] options = new string[feedback.feedbackGroups.Count];
            for (int i = 0; i < feedback.feedbackGroups.Count; i++)
                options[i] = feedback.feedbackGroups[i].feedbackGroupName;

            
            currentFeedbackGroup = EditorGUILayout.Popup(currentFeedbackGroup, options);

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            if (GUILayout.Button("Create Group", EditorStyles.miniButtonLeft))
            {
                isEditingName = true;
                EventFeedback.EventFeedbackGroup group = new EventFeedback.EventFeedbackGroup();
                group.feedbackGroupName = "New Feedback Group";
                feedback.feedbackGroups.Add(group);
                currentFeedbackGroup = feedback.feedbackGroups.Count - 1;
                tempName = feedback.feedbackGroups[currentFeedbackGroup].feedbackGroupName;
            }
            if (feedback.feedbackGroups.Count <= 1) GUI.enabled = false;
            if (GUILayout.Button("Delete Group", EditorStyles.miniButtonMid))
            {
                feedback.feedbackGroups.RemoveAt(currentFeedbackGroup);
                currentFeedbackGroup = 0;
            }
            GUI.enabled = true;
            if (GUILayout.Button("Rename Group", EditorStyles.miniButtonRight))
            {
                isEditingName = true;
                tempName = feedback.feedbackGroups[currentFeedbackGroup].feedbackGroupName;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
        }


        if (!feedback.isSetup) return;
        
        

        //feedback.feedbackGroups[currentFeedbackGroup].feedbackGroupName = EditorGUILayout.TextField("Feedback Group Name", feedback.feedbackGroups[currentFeedbackGroup].feedbackGroupName);

        List<FeedbackItem> toDestroy = new List<FeedbackItem>();

        EditorGUILayout.Space(10);

        if (feedback.feedbackGroups[currentFeedbackGroup].feedbackItems.Count == 0)
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

        foreach (FeedbackItem item in feedback.feedbackGroups[currentFeedbackGroup].feedbackItems)
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
                //item.DrawUI(feedback);
                if (EditorGUI.EndChangeCheck())
                {
                    if (PrefabUtility.GetPrefabInstanceStatus(feedback.gameObject) == PrefabInstanceStatus.NotAPrefab)
                    {
                        EditorUtility.SetDirty(feedback.gameObject);
                    }
                    else
                    {
                        if (!Application.isPlaying)
                        {
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }
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
            feedback.feedbackGroups[currentFeedbackGroup].feedbackItems.Remove(item);

        // Create the 'Add Feedback' button.
        string addFeedbackLabel = "Add Feedback";
        if (feedback.feedbackGroups[currentFeedbackGroup].feedbackItems.Count > 0) addFeedbackLabel = "Add More Feedback";
        if (GUILayout.Button(addFeedbackLabel))
        {
            GenericMenu menu = new GenericMenu();

            List<FeedbackItem> items = feedback.feedbackGroups[currentFeedbackGroup].feedbackItems;

            menu.AddItem (new GUIContent("Game Object - Create"), false,
                () => { AddItem(new CreateGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Destroy"), false,
                () => { AddItem(new DestroyGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Enable"), false,
                () => { AddItem(new EnableGameObject()); });

            menu.AddItem(new GUIContent("Game Object - Disable"), false,
                () => { AddItem(new DisableGameObject()); });

            menu.AddItem(new GUIContent("Renderer - Pulse Size"), false,
                () => { AddItem(new PulseSize()); });

            menu.AddItem(new GUIContent("Renderer - Flash Color"), false,
                () => { AddItem(new FlashColor()); });

            menu.AddItem(new GUIContent("Sound - Play"), false,
                () => { AddItem(new PlaySound()); });

            menu.AddItem(new GUIContent("Particle System - Emit"), false,
                () => { AddItem(new EmitParticles()); });

            menu.AddItem(new GUIContent("Animation - Play"), false,
                () => { AddItem(new PlayAnimation()); });

            menu.AddItem(new GUIContent("Animator - Set Trigger"), false,
                () => { AddItem(new SetAnimatorTrigger()); });

            menu.AddItem(new GUIContent("Animator - Set Variable Value"), false,
                () => { AddItem(new SetAnimatorVariable()); });

            menu.AddItem(new GUIContent("Camera - Flash"), false,
                () => { AddItem(new ScreenFlash()); });

            menu.AddItem(new GUIContent("Camera - Shake"), false,
                () => { AddItem(new CameraShake()); });

            menu.AddItem(new GUIContent("Camera - Add Shader Effect"), false,
                () => { AddItem(new ApplyCameraShader()); });

            menu.AddItem(new GUIContent("Camera - Remove Shader Effect"), false,
                () => { AddItem(new RemoveCameraShader()); });

            menu.AddItem(new GUIContent("Haptics - Vibrate Controller"), false,
                () => { AddItem(new VibrateController()); });

            menu.AddItem(new GUIContent("Haptics - Vibrate Mobile"), false,
                () => { AddItem(new VibrateMobile()); });

            menu.AddItem(new GUIContent("Video - Play Video"), false,
                () => { AddItem(new PlayVideo()); });

            menu.AddItem(new GUIContent("Video - Stop Video"), false,
                () => { AddItem(new StopVideo()); });

            menu.AddItem(new GUIContent("Post Processing - Flash Profile"), false,
                () => { AddItem(new PPFlashProfile()); });

            menu.ShowAsContext();

        }
    }

    protected virtual void AddItem (FeedbackItem item)
    {
        EventFeedback feedback = (EventFeedback)target;
        List<FeedbackItem> items = feedback.feedbackGroups[currentFeedbackGroup].feedbackItems;
        items.Add(item);
        EditorUtility.SetDirty((EventFeedback)target);
    }
}
