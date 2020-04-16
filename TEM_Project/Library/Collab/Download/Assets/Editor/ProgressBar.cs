using UnityEditor;

//will just take inputs from an outside source and displays the bar. It will not do any updating of the progress 
public class ProgressBar
{
    float m_Progress = default;
    public float Progress { set { m_Progress = value; } /*private get { return m_Progress; }*/ }

    string m_Title = default;
    public string TitleString { set { m_Title = value; } }

    string m_Information = default;
    public string InformationString { set { m_Information = value; } }

    public ProgressBar(
        float startingProgress = 0,
        string titleOfProgressBar = "Default Title of Progress Bar",
        string infoOfProgressBar = "Loading underneath progress bar"
        )
    {
        m_Progress = startingProgress;
        m_Title = titleOfProgressBar;
        m_Information = infoOfProgressBar;

    }

    public void ResetProgress(float startingProgress,
        string titleOfProgressBar = "Default Title of Progress Bar",
        string infoOfProgressBar = "Loading underneath progress bar")
    {
        m_Progress = startingProgress;
        m_Title = titleOfProgressBar;
        m_Information = infoOfProgressBar;
    }

    //Returns true if user clicks on cancel button or operation is over
    public bool Draw()
    {
        if (m_Progress < 1.0f)
        {
            if (EditorUtility.DisplayCancelableProgressBar(m_Title, m_Information, m_Progress))
            {
                EditorUtility.ClearProgressBar();
                return true;
            }

            return false;
        }
        else
        {
            EditorUtility.ClearProgressBar();
            return true;
        }
    }


}
