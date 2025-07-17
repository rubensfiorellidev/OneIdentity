using Microsoft.Graph;

namespace OneID.Application.Interfaces.Graph
{
    public interface IGraphServiceClientFactory
    {
        GraphServiceClient GetGraphServiceClient();
    }

}
