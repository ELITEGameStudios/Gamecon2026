using System.Collections.Generic;
using UnityEngine;

public abstract class WaveBase : ScriptableObject
{
    public SpawnProfileData[] profilesData;
    public bool finishedProcesses;
    public bool startedProcesses;
    public float timeBeforeFirstGate, prewaveTime;

    public abstract void Start();
    public abstract void WaveUpdate();
    public virtual void OnBeginSpawnProcess(){}
    public virtual void WaveEnd(){}
    public virtual void OnProfileFinishProcess(SpawnProfile self){}
    
    public void InternalStart()
    {
        Debug.Log("Ping");
        finishedProcesses = false;
        
        if(profilesData.Length == 0){
            Debug.LogAssertion(GetType() + " Cannot start, there are no spawn profiles in queue"); 
            EndWave(); 
            return;
        }

        if(timeBeforeFirstGate > 0){
            prewaveTime = timeBeforeFirstGate;
            startedProcesses = false;
        }
        else{
            Start();    
            startedProcesses = true;
        }
        
    }
    public void InternalWaveUpdate()
    {
        // Wave hasnt begun
        if (!startedProcesses){
            PrewaveTimer();
            return;
        }

        // Wave is in progress
        if (!finishedProcesses)
        {
            WaveUpdate();    
        }

        // Wave spawning is finished and is just awaiting the final shared condition
        else
        {
            WaveManager.Instance.CheckUniversalEndCondition();    
        }
    }

    void PrewaveTimer()
    {
        prewaveTime -= Time.deltaTime;
        if(prewaveTime <= 0){
            startedProcesses = true;
            Start();
        }
    }

    protected void NewSpawnProcess(SpawnProfileData data)
    {
        SpawnProfile profile = data.NewProfile(); 
        WaveManager.Instance.AddActiveSpawnProcess(profile);
        profile.OnFinishSpawnProcess += InternalOnProfileFinishProcess;
        profile.StartProcess();
        OnBeginSpawnProcess();
    }
    
    protected void InternalOnProfileFinishProcess(SpawnProfile finishedProfile){
        finishedProfile.OnFinishSpawnProcess -= InternalOnProfileFinishProcess;
        WaveManager.Instance.RemoveActiveSpawnProcess(finishedProfile);
        OnProfileFinishProcess(finishedProfile);
    }

    protected void TryEndWaveProcesses()
    {
        if(WaveManager.Instance.activeProfiles.Count == 0)
        {
            finishedProcesses = true;
        }
    }

    public void EndWave(){ // Formerly InternalWaveEnd, just renamed since in practice this might be more clear
        WaveEnd();
        (GameManager.Instance.entityManager as WaveManager).OnWaveEnd(); 
    }

}

public abstract class SequentialWave : WaveBase
{
    public int currentProfileIndex;

    public override void Start(){
        currentProfileIndex = 0;
        NewSpawnProcess(profilesData[currentProfileIndex]);
    }

    public virtual void TryInitiateNextSpawn()
    {
        if(currentProfileIndex+1 >= profilesData.Length){
            TryEndWaveProcesses();
            return;
        }

        currentProfileIndex++;
        NewSpawnProcess(profilesData[currentProfileIndex]);
    }
}

