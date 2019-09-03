using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICornerPanel : BasicManager<UICornerPanel>
{
    [SerializeField]
    private Button _buildButton;
    [SerializeField]
    private Text _buildButtonText;
    [SerializeField]
    private Text _powerText;
    private bool _isBuildingMode;
    private int _power;
    void Start()
    {
        _buildButton.onClick.AddListener(TurnBuildState);
        BuildingController.instance.OnStateChanged += UpdateUI;
        _power = 0;
    }

    void TurnBuildState()
    {
        if (_isBuildingMode)
        {
            _buildButtonText.text = "Build";
            _isBuildingMode = false;
        }
        else
        {
            _buildButtonText.text = "Exit";
            _isBuildingMode = true;
        }
        BuildingController.instance.SetBuildingMode(_isBuildingMode);
    }

    public void UpdatePower(int power)
    {
        _power += power;
        _powerText.text = "Power is " + _power;
    }

    void UpdateUI(BuildingState state)
    {
        if (state == BuildingState.Ready || state == BuildingState.Leaving)
            TurnBuildState();
    }
}
