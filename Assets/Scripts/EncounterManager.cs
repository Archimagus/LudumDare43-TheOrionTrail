using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public Encounters RandomEncounters;
    public Encounters OptionalEncounters;

    public IntReference Fuel;
    public IntReference Water;
    public IntReference Food;
    public IntReference People;
    public IntReference Fighters;

    public List<IntReference> FleetHealth;

    public EncounterReference _ActiveEncounter;

    private System.Random random = new System.Random();

    public void RandomEncounter()
    {
        if (RandomEncounters != null)
        {
            var availableEncounters = RandomEncounters.AvailableEncounters.Where(e => e.Encountered == false);
            if(availableEncounters.Count() == 0)
            {
                foreach(var enc in RandomEncounters.AvailableEncounters)
                {
                    enc.Encountered = false;
                }
                availableEncounters = RandomEncounters.AvailableEncounters;
            }

            var encounter = availableEncounters.Random();
            _ActiveEncounter.Value = encounter;

            //Debug.Log($"{encounter.Description} Selected.");
        }
    }

    public void OptionalEncounter()
    {
        if (OptionalEncounters != null)
        {
            var availableEncounters = OptionalEncounters.AvailableEncounters.Where(e => e.Encountered == false);
            if (availableEncounters.Count() == 0)
            {
                foreach (var enc in OptionalEncounters.AvailableEncounters)
                {
                    enc.Encountered = false;
                }
                availableEncounters = OptionalEncounters.AvailableEncounters;
            }

            var encounter = availableEncounters.Random();
            _ActiveEncounter.Value = encounter;
        }
    }

    public void ChoiceCausesDamage(DataVariable data)
    {
        var sideEffects = data.Data.Data;
        var damageSideEffects = sideEffects.Where(effect => effect.Key.ToLower().Contains("damage_chance"));
        var fighterSideEffects = sideEffects.Where(effect => effect.Key.ToLower().Contains("fighters_chance"));
        var otherSideEffects = sideEffects.Except(damageSideEffects).Except(fighterSideEffects);

        ProcessDamageEffects(damageSideEffects.ToList());
        ProcessFighterEffects(fighterSideEffects.ToList());
        ProcessEffects(otherSideEffects.ToList());
    }

    public void ChoiceMade(DataVariable data)
    {
        var sideEffects = data.Data.Data;
        ProcessEffects(sideEffects.ToList());
    }

    private void ProcessDamageEffects(List<KVP> sideEffects)
    {
        System.Random r = new System.Random();
        if(FleetHealth?.Count > 0)
        {
            var times = Mathf.Abs(sideEffects.FirstOrDefault(effect => effect.Key == "damage")?.Value??0);
            var prob = sideEffects.FirstOrDefault(effect => effect.Key.Contains("chance"))?.Value ?? 0;

            for(int i = 0; i < times; ++i)
            {
                if(r.Next(0, 101) < prob)
                {
                    FleetHealth.Where(x => x.Value > 0).Random().Value -= 1;
                }
            }
        }
    }

    private void ProcessFighterEffects(List<KVP> sideEffects)
    {
        System.Random r = new System.Random();
        var times = Mathf.Abs(sideEffects.FirstOrDefault(effect => effect.Key == "fighters")?.Value ?? 0);
        var prob = sideEffects.FirstOrDefault(effect => effect.Key.Contains("chance"))?.Value ?? 0;

        for(int i = 0; i < times; ++i)
        {
            if(r.Next(0, 101) < prob)
            {
                Fighters.Value -= 1;
            }
        }
    }

    private void ProcessEffects(List<KVP> sideEffects)
    {
        foreach(var effect in sideEffects)
        {
            string name = effect.Key.ToLower();

            if (name == "fuel") Fuel.Value += effect.Value;
            if (name == "food") Food.Value += effect.Value;
            if (name == "water") Water.Value += effect.Value;
            if (name == "people") People.Value += effect.Value;
			if (name == "fighters") Fighters.Value += effect.Value;
			if (name == "damage")
			{
				int count = 0;

				while (count < effect.Value)
				{
					FleetHealth.Where(x => x.Value > 0).Random().Value -= 1;
					count++;
				}
			}

			if (name == "repair") RepairFleet(effect.Value);
        }
    }

    private void RepairFleet(int count)
    {
        for(int i = 0; i < count; ++i)
        {
			var shipHealth = FleetHealth.Where(x => x.Value > 0 && x.Value < 5).Random();
			if(shipHealth != null)
				shipHealth.Value += 1;
        }
    }
}