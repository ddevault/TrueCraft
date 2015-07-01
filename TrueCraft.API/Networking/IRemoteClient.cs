using System;
using TrueCraft.API.World;
using TrueCraft.API.Entities;
using TrueCraft.API.Windows;
using TrueCraft.API.Server;

namespace TrueCraft.API.Networking
{
    public interface IRemoteClient
    {
        /// <summary>
        /// Minecraft stream used to communicate with this client.
        /// </summary>
        IMinecraftStream MinecraftStream { get; }
        /// <summary>
        /// Returns true if this client has data pending in the network stream.
        /// </summary>
        bool DataAvailable { get; }
        /// <summary>
        /// The world this client is present in.
        /// </summary>
        IWorld World { get; }
        /// <summary>
        /// The entity associated with this client.
        /// </summary>
        IEntity Entity { get; }
        /// <summary>
        /// This client's inventory.
        /// </summary>
        IWindow Inventory { get; }
        /// <summary>
        /// The username of the connected client. May be null if not yet ascertained.
        /// </summary>
        string Username { get; }
        /// <summary>
        /// The slot index this user has selected in their hotbar.
        /// </summary>
        short SelectedSlot { get; }
        /// <summary>
        /// The item stack at the slot the user has selected in their hotbar.
        /// </summary>
        ItemStack SelectedItem { get; }
        /// <summary>
        /// The server this user is playing on.
        /// </summary>
        IMultiplayerServer Server { get; }
        /// <summary>
        /// If true, this client will be sent logging information as chat messages.
        /// </summary>
        bool EnableLogging { get; set; }
        /// <summary>
        /// The time the user is expected to complete the active digging operation,
        /// depending on what kind of block they are mining and what tool they're using
        /// to do it with.
        /// </summary>
        DateTime ExpectedDigComplete { get; set; }
        /// <summary>
        /// True if this client has been disconnected. You should cease sending packets and
        /// so on, this client is just waiting to be reaped.
        /// </summary>
        bool Disconnected { get; }

        /// <summary>
        /// Loads player data from disk for this client.
        /// </summary>
        bool Load();
        /// <summary>
        /// Saves player data to disk for this client.
        /// </summary>
        void Save();
        /// <summary>
        /// Queues a packet to be sent to this client.
        /// </summary>
        void QueuePacket(IPacket packet);
        /// <summary>
        /// Disconnects this client from the server.
        /// </summary>
        void Disconnect();
        /// <summary>
        /// Sends a chat message to this client.
        /// </summary>
        void SendMessage(string message);
        /// <summary>
        /// If logging is enabled, sends your message to the client as chat.
        /// </summary>
        void Log(string message, params object[] parameters);
        /// <summary>
        /// Opens a window on the client. This sends the appropriate packets and tracks
        /// this window as the currently open window.
        /// </summary>
        void OpenWindow(IWindow window);
    }
}