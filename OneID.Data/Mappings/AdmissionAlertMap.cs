using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Mappings
{
    internal class AdmissionAlertMap : IEntityTypeConfiguration<AdmissionAlert>
    {
        public void Configure(EntityTypeBuilder<AdmissionAlert> builder)
        {
            builder.ToTable("tb_hwid_admission_alert");

            builder.HasKey(x => x.Id);
        }
    }
}
