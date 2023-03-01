using ZoaIds.Shared.ExternalDataModels;
using ZoaIds.Shared.Models;

namespace ZoaIds.Client.Services;

public class StateContainer
{
    public event Action? OnDatafeedUpdate;
    public event Action? OnDataChange;
	public event Action? OnTimeChange;

	private VatsimJsonRoot? _vatsimDatafeed;
	public VatsimJsonRoot? VatsimDatafeed
	{
		get => _vatsimDatafeed;
		set
		{
			_vatsimDatafeed = value;
			NotifyDatafeedUpdated();
		}
	}

	private Airport? _selectedTowerCabAirport;
	public Airport? SelectedTowerCabAirport
	{
		get => _selectedTowerCabAirport;
		set
		{
			_selectedTowerCabAirport = value;
			NotifyDataStateChanged();
		}
	}

	private DateTime _now = DateTime.UtcNow;
	public DateTime Now
	{
		get => _now;
		set
		{
			_now = value;
			NotifyTimeStateChanged();
		}
	}

    private void NotifyDatafeedUpdated() => OnDatafeedUpdate?.Invoke();
    private void NotifyDataStateChanged() => OnDataChange?.Invoke();
	private void NotifyTimeStateChanged() => OnTimeChange?.Invoke();
}
