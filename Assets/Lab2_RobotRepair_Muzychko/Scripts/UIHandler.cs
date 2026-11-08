
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    public float displayTime = 4.0f;

    private VisualElement m_NonPlayerDialogue;
    private Label m_DialogueLabel;
    private float m_TimerDisplay;

    private VisualElement m_Healthbar;
    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("UIHandler: UIDocument не знайдено!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;

        m_Healthbar = root.Q<VisualElement>("HealthBar");
        m_NonPlayerDialogue = root.Q<VisualElement>("NPCDialogue");
        m_LoseScreen = root.Q<VisualElement>("LoseScreenContainer");
        m_WinScreen = root.Q<VisualElement>("WinScreenContainer");

        if (m_NonPlayerDialogue == null)
        {
            Debug.LogError("NPCDialogue НЕ ЗНАЙДЕНО!");
        }
        else
        {
            m_NonPlayerDialogue.style.display = DisplayStyle.None;
            m_DialogueLabel = m_NonPlayerDialogue.Q<Label>();

            Debug.Log("ТЕКСТ LABEL: " + (m_DialogueLabel == null ? "НЕ ЗНАЙДЕНО" : "ЗНАЙДЕНО"));

            if (m_DialogueLabel == null)
                Debug.LogError("Label усередині NPCDialogue не знайдено!");
        }

        SetHealthValue(1.0f);

        m_TimerDisplay = -1.0f;
    }

    public void SetHealthValue(float percentage)
    {
        if (m_Healthbar != null)
            m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;

            if (m_TimerDisplay <= 0)
            {
                if (m_NonPlayerDialogue != null)
                    m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void DisplayDialogue()
    {
        Debug.Log("DisplayDialogue викликано!");

        if (m_NonPlayerDialogue == null || m_DialogueLabel == null)
        {
            Debug.LogError("Не знайдено діалогове вікно або Label!");
            return;
        }

        m_DialogueLabel.text = "Привіт! Радий тебе бачити!";
        Debug.Log("ТЕКСТ В LABEL: " + m_DialogueLabel.text);
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;

        m_TimerDisplay = displayTime;
    }

    public void DisplayWinScreen()
    {
        if (m_WinScreen != null)
            m_WinScreen.style.opacity = 1.0f;
    }

    public void DisplayLoseScreen()
    {
        if (m_LoseScreen != null)
            m_LoseScreen.style.opacity = 1.0f;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
