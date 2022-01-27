using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Attributes { Strength, Speed, Agility, Stamina };

public class DataHolder : GenericSingletonClass<DataHolder>
{
    public const string COIN_ICON = "<sprite=0>";
    public const float SELL_RATIO = 0.5f;
    public const int DEFAULT_CURRENCY = 2000;

    public static Dictionary<FighterData, TrainerData> fightersInTraining;
    public static List<int> usedTrainingSpots = new List<int>();
    public static bool startOfSession = true;
    public static int fightsWon;
    public static int currency;
    public static FighterData selectedFighter;
    public static FighterContainer ownedChampions;
    public static FighterContainer unownedChampions;

    public static FighterContainer DefaultUnownedChampions()
    {
        FighterContainer _container = new FighterContainer();
        _container.Add(new FighterData("Fighter I", 60, 20, 10, 15, "sword", "square"));
        _container.Add(new FighterData("Fighter II", 20, 10, 60, 15, "mace", "hex"));
        _container.Add(new FighterData("Fighter III", 10, 10, 20, 40, "club", "round"));
        _container.Add(new FighterData("Fighter IV", 25, 25, 25, 17, "sword", "square"));
        _container.Add(new FighterData("Fighter V", 25, 25, 20, 17, "mace", "hex"));
        _container.Add(new FighterData("Fighter VI", 25, 30, 20, 17, "sword", "round"));
        _container.Add(new FighterData("Fighter VII", 25, 40, 20, 17, "mace", "square"));
        _container.Add(new FighterData("Fighter VIII", 25, 50, 20, 17, "club", "hex"));
        _container.Add(new FighterData("Fighter IX", 10, 60, 20, 15));
        _container.Add(new FighterData("Fighter X", 10, 100, 100, 15));
        return _container;
    }

    public static FighterContainer DefaultOwnedChampions()
    {
        FighterContainer _container = new FighterContainer();
        return _container;
    }

    private static void RemoveActiveFighter()
    {
        ownedChampions.Remove(selectedFighter);
        selectedFighter = null;
    }

    public static void ActiveFighterTraining(TrainerData _trainerData)
    {
        if (fightersInTraining == null) {
            fightersInTraining = new Dictionary<FighterData, TrainerData>();
        }
        ownedChampions.Remove(selectedFighter);
        fightersInTraining.Add(selectedFighter, _trainerData);
        selectedFighter = null;
    }

    public static bool FindTrainerData(int _id, out TrainerData _trainerData)
    {
        _trainerData = null;
        if (fightersInTraining == null) {
            return false;
        }
        for (int i = fightersInTraining.Count - 1; i >= 0; i--)
        {
            KeyValuePair<FighterData, TrainerData> _pair = fightersInTraining.ElementAt(i);
            if (_pair.Value.ID == _id)
            {
                _trainerData = _pair.Value;
                return true;
            }
        }
        return false;
    }

    private static bool SpotIsUsed(int _id)
    {
        for (int i = fightersInTraining.Count - 1; i >= 0; i--)
        {
            KeyValuePair<FighterData, TrainerData> _pair = fightersInTraining.ElementAt(i);
            if (_pair.Value.ID == _id)
            {
                return true;
            }
        }
        return false;
    }

    public static string UsedFighterName(int _id)
    {
        for (int i = fightersInTraining.Count - 1; i >= 0; i--)
        {
            KeyValuePair<FighterData, TrainerData> _pair = fightersInTraining.ElementAt(i);
            if (_pair.Value.ID == _id)
            {
                return _pair.Key.Name;
            }
        }
        return "";
    }

    private static void RemoveTrainingFighter(KeyValuePair<FighterData, TrainerData> _pair)
    {
        ownedChampions.Add(_pair.Key);
        fightersInTraining.Remove(_pair.Key);
        usedTrainingSpots.Remove(_pair.Value.ID);
    }

    private static int RandomAddedValue()
    {
        int _value = Random.Range(10, 21);
        return _value;
    }
}
