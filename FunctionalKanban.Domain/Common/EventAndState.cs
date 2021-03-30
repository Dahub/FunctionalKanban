namespace FunctionalKanban.Domain.Common
{
    public record EventAndState<E, S>(E @event, S state) where E:Event where S:State;
}
