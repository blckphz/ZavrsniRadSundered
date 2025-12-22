using UnityEngine;

public interface IAggroCheck
{
    bool IsAggroed { get; }

    void Aggro(GameObject target);
}
