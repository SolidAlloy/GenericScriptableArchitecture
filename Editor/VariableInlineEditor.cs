namespace GenericScriptableArchitecture.Editor
{
    internal class VariableInlineEditor : VariableEditorBase
    {
        protected override void DrawFields()
        {
            if (InPlayMode)
            {
                DrawCurrentValue();
                DrawPreviousValue();
            }
            else
            {
                DrawInitialValue();
            }
        }
    }
}