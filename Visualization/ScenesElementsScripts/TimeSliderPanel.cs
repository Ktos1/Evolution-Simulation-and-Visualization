using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;


public partial class TimeSliderPanel : Panel
{
    [Export] private Button _slidingButton;
    [Export] private Label _actualTimeLabel;
    [Export] private Label _totalTimeLabel;
    [Export] private HSlider _timeSlider;

    private int _totalTicksNumber = BinReader.TotalTicksNumber;
    private int _currentTickNumber => BinReader.CurrentTickNumber;


    public override void _Ready()
    {
        _slidingButton.Toggled += OnSlidingButtonToggled;
        _totalTimeLabel.Text = $"{GetTimeFromTicks(_totalTicksNumber)}";
        _timeSlider.MaxValue = _totalTicksNumber - 1;
    }

    public override void _Process(double delta)
    {
        _actualTimeLabel.Text = $"{GetTimeFromTicks(_currentTickNumber)}";
        _timeSlider.Value = _currentTickNumber;
    }

    private void OnSlidingButtonToggled(bool newState)
    {
        if (newState) Position += new Vector2(0, -52);
        else Position += new Vector2(0, 52);
    }

    private string GetTimeFromTicks(int ticks)
    {
        float totalSeconds = ticks * CommonSettings.TICK_DURATION;
        ushort hours = 0;
        ushort minutes = 0;
        ushort seconds = 0;
        if (totalSeconds < 1)
        {
            seconds = 1;
        }
        else
        {
            hours = (ushort)(totalSeconds / 3600);
            totalSeconds -= hours * 3600;
            minutes = (ushort)(totalSeconds / 60);
            totalSeconds -= minutes * 60;
            seconds = (ushort)totalSeconds;
        }
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
