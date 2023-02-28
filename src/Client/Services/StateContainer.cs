using ZoaIds.Shared.ExternalDataModels;
using ZoaIds.Shared.Models;

namespace ZoaIds.Client.Services;

public class StateContainer
{
	public event Action? OnChange;

	private VatsimJsonRoot? _vatsimDatafeed;
	public VatsimJsonRoot? VatsimDatafeed
	{
		get => _vatsimDatafeed;
		set
		{
			_vatsimDatafeed = value;
			NotifyStateChanged();
		}
	}

	private Airport? _selectedTowerCabAirport;
	public Airport? SelectedTowerCabAirport
	{
		get => _selectedTowerCabAirport;
		set
		{
			_selectedTowerCabAirport = value;
			NotifyStateChanged();
		}
	}

	private void NotifyStateChanged() => OnChange?.Invoke();
}
