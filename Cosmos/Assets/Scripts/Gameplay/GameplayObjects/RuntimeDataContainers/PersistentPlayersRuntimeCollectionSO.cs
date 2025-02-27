using Cosmos.Infrastructure;
using Cosmos.Utilities;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects
{
    /// <summary>
    /// A runtime list of <see cref="PersistentPlayer"/> objects that is populated both on clients and server.
    /// </summary>
    [CreateAssetMenu(fileName = "PersistentPlayersRuntimeData", menuName = "ScriptableObjects/RuntimeDatas/PersistentPlayer")]
    public class PersistentPlayersRuntimeCollectionSO : RuntimeCollectionSO<PersistentPlayer>
    {
        public bool TryGetPlayer(ulong clientId, out PersistentPlayer persistentPlayer)
        {
            for (int i = 0, count = Items.Count; i < count; ++i)
            {
                if (Items[i].OwnerClientId == clientId)
                {
                    persistentPlayer = Items[i];
                    return true;
                }
            }

            persistentPlayer = null;
            return false;
        }

        /// <summary>
        /// Get the player name of the given client id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public bool TryGetPlayerName(ulong clientId, out FixedPlayerName playerName)
        {
            if (TryGetPlayer(clientId, out var persistentPlayer))
            {
                playerName = persistentPlayer.NetworkNameState.Name.Value;
                return true;
            }

            playerName = null;
            return false;
        }
    }

}
