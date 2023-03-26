using UnityEngine;

public class ImageSliderBarNumStoreDisplay : StoreDisplay
{
    [SerializeField] private NumStore store;
    [SerializeField] private ImageSliderBar bar;

    private void Update()
    {
        bar.Set(store.GetValue(), store.MaxValue);
    }
}
