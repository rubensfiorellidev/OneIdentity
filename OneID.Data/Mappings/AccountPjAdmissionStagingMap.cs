using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Mappings
{
    internal class AccountPjAdmissionStagingMap : IEntityTypeConfiguration<AccountPjAdmissionStaging>
    {
        public void Configure(EntityTypeBuilder<AccountPjAdmissionStaging> builder)
        {
            builder.ToTable("tb_oneid_account_pj_admission_staging");

            builder.HasKey(x => x.CorrelationId);

            builder.Property(x => x.CorrelationId)
                   .IsRequired();

            builder.Property(x => x.FullName)
                   .HasMaxLength(300);

            builder.Property(x => x.SocialName)
                   .HasMaxLength(150);

            builder.Property(x => x.Cpf)
                   .HasMaxLength(14);

            builder.Property(x => x.CpfHash)
                   .HasMaxLength(128);

            builder.Property(x => x.ForeignWorker)
                   .IsRequired();

            builder.Property(x => x.FiscalNumberIdentity)
                   .HasMaxLength(50);

            builder.Property(x => x.FiscalNumberIdentityHash)
                   .HasMaxLength(128);

            builder.Property(x => x.StartDate)
                   .HasColumnType("date");

            builder.Property(x => x.EndDate)
                   .HasColumnType("date");

            builder.Property(x => x.ContractorCnpj)
                   .HasMaxLength(18);

            builder.Property(x => x.ContractorName)
                   .HasMaxLength(200);

            builder.Property(x => x.PositionHeldId)
                   .HasMaxLength(100);

            builder.Property(x => x.RegionalId)
                   .HasMaxLength(100);

            builder.Property(x => x.RegionalName)
                   .HasMaxLength(150);

            builder.Property(x => x.RegionalState)
                   .HasMaxLength(2);

            builder.Property(x => x.ManagerId)
                   .HasMaxLength(100);

            builder.Property(x => x.CostCenterId)
                   .HasMaxLength(100);

            builder.Property(x => x.DepartmentId)
                   .HasMaxLength(100);

            builder.Property(x => x.Login)
                   .HasMaxLength(100);

            builder.Property(x => x.LoginHash)
                   .HasMaxLength(128);

            builder.Property(x => x.EmailAccess)
                   .IsRequired();

            builder.Property(x => x.PersonalEmail)
                   .HasMaxLength(150);

            builder.Property(x => x.PersonalEmailHash)
                   .HasMaxLength(128);

            builder.Property(x => x.CorporateEmail)
                   .HasMaxLength(150);

            builder.Property(x => x.CorporateEmailHash)
                   .HasMaxLength(128);

            builder.Property(x => x.VpnAccess)
                   .IsRequired();

            builder.Property(x => x.Comments)
                   .HasMaxLength(500);

            builder.Property(x => x.CreatedBy)
                   .HasMaxLength(100);

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("timestamptz")
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(x => x.CpfHash).HasDatabaseName("idx_admission_staging_cpfhash");
            builder.HasIndex(x => x.LoginHash).HasDatabaseName("idx_admission_staging_loginhash");
            builder.HasIndex(x => x.CorrelationId).IsUnique().HasDatabaseName("idx_admission_staging_correlation");
        }

    }
}
