using GuardianTalesGuildRaidUpdater.Models;

namespace GuardianTalesGuildRaidUpdater.Services.QuartzJobs
{
    [DisallowConcurrentExecution]
    public class RandomImageJob : IJob
    {
        private readonly ContentImageIndex imageContainer;

        public RandomImageJob(ContentImageIndex imageContainer)
        {
            this.imageContainer = imageContainer;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                int currentRandomInt = RandomNumberGenerator.GetInt32(0, imageContainer.Images.Count);

                if (imageContainer.IdxQueue.Count >= 5)
                {
                    imageContainer.IdxQueue.Clear();
                }

                imageContainer.IdxQueue.Enqueue(currentRandomInt);
            });
        }
    }
}
