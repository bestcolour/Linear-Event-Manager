/// <summary>
/// So in photoshop, there is the ability to allow the user to control z to undo
/// previous actions. So this is what the Command pattern allows for.
/// 
/// I'll just define actions that are reversible right here so that i wont forget to include them
///
/// 1) Deletion of Nodes
/// 2) Adding of Nodes
/// 3) Assigning of in out points of the node
/// 4) Referencing of the scene variables into the editor window 
/// </summary>

namespace LEM_Editor
{

    public interface INodeCommand
    {
        void Execute();
        void Undo();
        void Redo();

        //To be called when history exceeds its memory n begin overwriting old commands or when you undo a command and then dont press redo, instead you act out a new command
        void OnClear();
        int CommandType { get; }

    }

}