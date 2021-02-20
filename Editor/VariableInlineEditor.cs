namespace GenericScriptableArchitecture.Editor
{
    internal class VariableInlineEditor : VariableEditorBase
    {
        protected override void DrawFields()
        {
            if (ApplicationUtil.InPlayMode)
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