using UnityEngine;

public class PositionSaver : Saver
{
    public Transform transformToSave;

    protected override string setKey()
    {
        return transformToSave.name + transformToSave.GetType().FullName + uniqueIdentifier;
    }

    protected override void save()
    {
        saveData.save(key, transformToSave.position);
    }

    protected override void load()
    {
        Vector3 position = Vector3.zero;
        if (saveData.load(key, ref position))
            transformToSave.position = position;
    }
}
