using System;
using _Scripts.Factions;
using _Scripts.Tiles;
using _Scripts.Utility;
using UniRx;
using Zenject;

namespace _Scripts.Growth
{
    public class GrowthSystem : Subscription
    {
        [Inject] private ITileModel _model;
        [Inject] private ISpreadData _data;
        [Inject] private Faction _faction;
        
        public override void Initialize()
        {
            _model.Intensity(_faction).Subscribe(StartGrowth).AddTo(_disposer);
        }

        private void StartGrowth(int degree)
        {
            if (degree == 0 || degree >= 5)
                return;

            var timeTilNextStage = _data.GrowthDuration(_faction, degree);
            Observable.Timer(TimeSpan.FromSeconds(timeTilNextStage))
                .Where(_ => _model.Intensity(_faction).Value == degree).Take(1)
                .Subscribe(_ => _model.Intensity(_faction).Value++).AddTo(_disposer);
        }
    }
}