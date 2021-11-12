using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SatorImaging.UIEverything
{
    public class UIEverything : MonoBehaviour
    {
        public const string INTERNAL_PREFIX = "_UIEverything_";
        public const string METHOD_PREFIX = "On";
        public const string CLICK_SUFFIX = "Click";
        public const string CHANGED_SUFFIX = "Changed";
        public const string INTERACTIVE_SUFFIX = "ChangedInteractive";


        public bool syncMode = false;
        public Slider slider;
        public InputField inputField;



#if UNITY_EDITOR
        void Reset() => Initialize();
        void OnValidate() => Initialize();

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            // OnoVvalidate called when play starts
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            slider = GetComponent<Slider>();
            inputField = GetComponent<InputField>();
            syncMode = (slider || inputField);
            if (syncMode) return;


            // event handler mode.
            var targetTypes = new Type[]{
            typeof(Button),
            typeof(Slider),
            typeof(Toggle),
            typeof(InputField),
            typeof(Scrollbar),
            typeof(Dropdown),
            typeof(ScrollRect)
        };

            // first, collect scroll view's scrollbars.
            var ignoredTargets = new List<Selectable>();
            foreach (ScrollRect sr in FindObjectsOfType(typeof(ScrollRect), true))
            {
                if (sr.horizontalScrollbar) ignoredTargets.Add(sr.horizontalScrollbar);
                if (sr.verticalScrollbar) ignoredTargets.Add(sr.verticalScrollbar);
            }


            // do it.
            foreach (var type in targetTypes)
            {
                var methodName = INTERNAL_PREFIX + METHOD_PREFIX + type.Name;
                foreach (var target in FindObjectsOfType(type, false)) // ignore invislble controls. there are templates.
                {
                    if (ignoredTargets.Contains(target as Selectable))
                    {
                        //Debug.Log($"{target} is ignored. thank you.");
                        continue;
                    }


                    if (typeof(Button) == type)
                    {
                        SetEventCallback(target, "m_OnClick", methodName + CLICK_SUFFIX);
                    }
                    else if (typeof(Toggle) == type)
                    {
                        SetEventCallback(target, "onValueChanged", methodName + CHANGED_SUFFIX);
                    }
                    else if (typeof(InputField) == type)
                    {
                        SetEventCallback(target, "m_OnEndEdit", methodName + CHANGED_SUFFIX);
                        SetEventCallback(target, "m_OnValueChanged", methodName + INTERACTIVE_SUFFIX);
                    }
                    else
                    {
                        SetEventCallback(target, "m_OnValueChanged", methodName + CHANGED_SUFFIX);
                    }
                }
            }

        }//
#endif


#if UNITY_EDITOR
        void SetEventCallback(UnityEngine.Object target, string eventPropertyName, string methodName)
        {
            // hack!
            // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/UnityEventDrawer.cs#L685
            //
            // m_PersistentCalls
            //   m_Calls = array
            //     m_Target = eventhandler script reference, not gameObject
            //     m_TargetAssemblyTypeName = eventhander script assembly name
            //     m_MethodName = string, name of event hander method
            //     m_Mode = (int)PersistentListenerMode(.Object)
            //     m_Arguments
            //       m_ObjectArgument = target object reference
            //       m_ObjectArgumentAssemblyTypeName = assembly type name
            //       m_IntArgument: 0
            //       m_FloatArgument: 0
            //       m_StringArgument:
            //       m_BoolArgument: 0
            //     m_CallState = (int)UnityEventCallState
            //
            var targetSO = new SerializedObject(target);
            var rootSP = targetSO.FindProperty(eventPropertyName);
            rootSP = rootSP.FindPropertyRelative("m_PersistentCalls");
            var callsSP = rootSP.FindPropertyRelative("m_Calls");

            // check existance
            var targetIndex = -1;
            for (var i = 0; i < callsSP.arraySize; i++)
            {
                var sp = callsSP.GetArrayElementAtIndex(i);

                // AssemblyQualifiedName doesn't match, stored data strips versions, etc.
                if (this == sp.FindPropertyRelative("m_Target").objectReferenceValue
                && methodName == sp.FindPropertyRelative("m_MethodName").stringValue)
                {
                    //Debug.Log($"{target.name}: callback is already set. updated.");
                    targetIndex = i;
                    return;
                }
            }

            // do it
            if (targetIndex < 0)
            {
                callsSP.InsertArrayElementAtIndex(0);
                targetIndex = 0;
            }

            var eventSP = callsSP.GetArrayElementAtIndex(targetIndex);
            eventSP.FindPropertyRelative("m_Target").objectReferenceValue = this;
            eventSP.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue = typeof(UIEverything).AssemblyQualifiedName;
            eventSP.FindPropertyRelative("m_MethodName").stringValue = methodName;
            eventSP.FindPropertyRelative("m_Mode").intValue = (int)PersistentListenerMode.Object;
            eventSP.FindPropertyRelative("m_CallState").intValue = (int)UnityEventCallState.RuntimeOnly;

            var argsSP = eventSP.FindPropertyRelative("m_Arguments");
            argsSP.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = target;
            argsSP.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = target.GetType().AssemblyQualifiedName;

            targetSO.ApplyModifiedProperties();
        }
#endif



        public void Synchronize_Slider_to_Text(Text targetText) => SyncSliderToText(targetText, "");
        public void Synchronize_Slider_to_Text_1_0(Text targetText) => SyncSliderToText(targetText, "F1");
        public void Synchronize_Slider_to_Text_1_00(Text targetText) => SyncSliderToText(targetText, "F2");
        public void Synchronize_Slider_to_Text_1_000(Text targetText) => SyncSliderToText(targetText, "F3");
        void SyncSliderToText(Text targetText, string option)
        {
            if (targetText && slider)
                targetText.text = slider.value.ToString(option);
        }


        public void Synchronize_Slider_to_InputField(InputField targetInputField)
        {
            if (targetInputField && slider)
                targetInputField.text = slider.value.ToString();
            //Debug.Log($"{nameof(SyncSliderToInputField)}: Loop Check.");
        }


        public void Synchronize_InputField_to_Slider(Slider targetSlider)
        {
            if (targetSlider && inputField)
            {
                float value;
                if (float.TryParse(inputField.text, out value))
                {
                    targetSlider.value = value;
                }
            }
            //Debug.Log($"{nameof(SyncInputFieldToSlider)}: Loop Check.");
        }




        public void _UIEverything_OnButtonClick(Button target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + CLICK_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }

        public void _UIEverything_OnInputFieldChanged(InputField target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + CHANGED_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }

        public void _UIEverything_OnInputFieldChangedInteractive(InputField target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + INTERACTIVE_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }



        public void _UIEverything_OnToggleChanged(Toggle target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + CHANGED_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }

        public void _UIEverything_OnSliderChanged(Slider target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + CHANGED_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }

        public void _UIEverything_OnScrollbarChanged(Scrollbar target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + CHANGED_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }

        public void _UIEverything_OnDropdownChanged(Dropdown target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + CHANGED_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }

        public void _UIEverything_OnScrollRectChanged(ScrollRect target)
        {
            SendMessage(METHOD_PREFIX + target.GetType().Name + CHANGED_SUFFIX, target, SendMessageOptions.DontRequireReceiver);
        }


    }//class



#if UNITY_EDITOR
    [CustomEditor(typeof(UIEverything))]
    public class UIEverythingInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            bool syncMode = (target as UIEverything).syncMode;

            // component mode.
            if (syncMode)
            {
                base.OnInspectorGUI();
                return;
            }

            // event hander mode.
            if (GUILayout.Button("Update UI Events", GUILayout.Height(64)))
            {
                (target as UIEverything).Initialize();
                Debug.Log(target.name + ": rebuild was succeessfully done.");
            }

            //base.OnInspectorGUI();
        }
    }//class
#endif


}//namespace
