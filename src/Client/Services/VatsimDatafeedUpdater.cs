using ZoaIds.Client.ApiClients;

namespace ZoaIds.Client.Services;

public class VatsimDatafeedUpdater
{
    private readonly VatsimApiClient _apiClient;
	private readonly StateContainer _stateContainer;
    private readonly Timer _timer;
    private bool _fetchInProgress = false;
	private int _updateIntervalSeconds;

	public bool IsUpdateLoopRunning { get; private set; } = false;

    public VatsimDatafeedUpdater(VatsimApiClient apiClient, StateContainer stateContainer, int updateIntervalSeconds = 30)
    {
        _apiClient = apiClient;
        _stateContainer = stateContainer;
        _updateIntervalSeconds = updateIntervalSeconds;
        _timer = new Timer(FetchDatafeedAndUpdateState);
    }

    private async void FetchDatafeedAndUpdateState(Object? stateInfo)
    {
        // Use _fetchInProgress as a lock so we don't have overlapping fetches
        if (!_fetchInProgress)
        {
			_fetchInProgress = true;
			if (_apiClient is not null)
			{
                _stateContainer.VatsimDatafeed = await _apiClient.GetDatafeed();
			}
			_fetchInProgress = false;
		}
    }

    public bool StartUpdateLoop()
    {
        if (IsUpdateLoopRunning)
        {
            return true;
        }
        else
        {
			// Change returns true if timer settings change was successful
			IsUpdateLoopRunning = _timer.Change(0, _updateIntervalSeconds * 1000);
            return IsUpdateLoopRunning;
        }
    }

    public bool StopUpdateLoop()
    {
        if (!IsUpdateLoopRunning)
        {
            return true;
        }
        else
        {
			// Change returns true if timer settings change was successful
			IsUpdateLoopRunning = !_timer.Change(Timeout.Infinite, Timeout.Infinite);
            return !IsUpdateLoopRunning;
        }
    }
}
