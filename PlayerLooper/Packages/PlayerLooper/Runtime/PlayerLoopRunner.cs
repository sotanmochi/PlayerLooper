using System;
using System.Collections.Generic;

namespace UnityPlayerLooper
{
    public class PlayerLoopItem
    {
        public object Target;
        public Action Action;
    }

    internal class PlayerLoopRunner
    {
        public int ItemCount => _playerLoopItemList.Count;

        readonly List<PlayerLoopItem> _playerLoopItemList = new List<PlayerLoopItem>();
        readonly Dictionary<object, PlayerLoopItem> _playerLoopItemDictionary = new Dictionary<object, PlayerLoopItem>();
        readonly bool _autoClear;

        public PlayerLoopRunner(bool autoClear = false)
        {
            _autoClear = autoClear;
        }

        public void Run()
        {
            foreach (var item in _playerLoopItemList)
            {
                item.Action?.Invoke();
            }

            if (_autoClear)
            {
                _playerLoopItemList.Clear();
                _playerLoopItemDictionary.Clear();
            }
        }

        public void Register(object target, Action action)
        {
            if (_playerLoopItemDictionary.ContainsKey(target))
            {
                return;
            }

            PlayerLoopItem playerLoopItem = new PlayerLoopItem(){ Target = target, Action = action };
            _playerLoopItemList.Add(playerLoopItem);
            _playerLoopItemDictionary.Add(playerLoopItem.Target, playerLoopItem);
        }

        public void Unregister(object target)
        {
            if (!_playerLoopItemDictionary.ContainsKey(target))
            {
                return;
            }

            int targetIndex = _playerLoopItemList.FindIndex(item => item.Target == target);
            if (targetIndex != -1)
            {
                _playerLoopItemList.RemoveAt(targetIndex);
            }

            _playerLoopItemDictionary.Remove(target);
        }

        public void Clear()
        {
            _playerLoopItemList.Clear();
            _playerLoopItemDictionary.Clear();
        }
    }
}