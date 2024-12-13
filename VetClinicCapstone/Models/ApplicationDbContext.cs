using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VetClinicCapstone.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public virtual DbSet<AdminInfoTbl> AdminInfoTbls { get; set; }
        public virtual DbSet<AdminTbl> AdminTbls { get; set; }
        public virtual DbSet<AppointmentScheduleTbl> AppointmentScheduleTbls { get; set; }
        public virtual DbSet<AppointmentTbl> AppointmentTbls { get; set; }
        public virtual DbSet<ConsultationTbl> ConsultationTbls { get; set; }
        public virtual DbSet<EventsTbl> EventsTbls { get; set; }
        public virtual DbSet<LaboratoryTbl> LaboratoryTbls { get; set; }
        public virtual DbSet<OwnerTbl> OwnerTbls { get; set; }
        public virtual DbSet<PatientTbl> PatientTbls { get; set; }
        public virtual DbSet<PrescriptionDetailTbl> PrescriptionDetailTbls { get; set; }
        public virtual DbSet<PrescriptionTbl> PrescriptionTbls { get; set; }
        public virtual DbSet<ServiceTbl> ServiceTbls { get; set; }
        public virtual DbSet<TreatmentTbl> TreatmentTbls { get; set; }
        public virtual DbSet<UserTbl> UserTbls { get; set; }

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-1NUKKEE;Initial Catalog=VetClinic;User ID=sa;Password=@Console05;Trust Server Certificate=True;Command Timeout=300");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminInfoTbl>(entity =>
            {
                entity.HasKey(e => e.AdminInfo_ID)
                    .HasName("PK_DoctorTbl");

                entity.Property(e => e.Barangay).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Firstname).IsUnicode(false);

                entity.Property(e => e.Lastname).IsUnicode(false);

                entity.Property(e => e.Middlename).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.Province).IsUnicode(false);

                entity.Property(e => e.Role).IsUnicode(false);

                entity.Property(e => e.Sex).IsUnicode(false);

                entity.Property(e => e.Street).IsUnicode(false);
            });

            modelBuilder.Entity<AdminTbl>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue((short)0);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.Username).IsUnicode(false);

                entity.HasOne(d => d.AdminInfo)
                    .WithMany(p => p.AdminTbls)
                    .HasForeignKey(d => d.AdminInfo_ID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdminTbl_AdminInfoTbl");
            });

            modelBuilder.Entity<AppointmentScheduleTbl>(entity =>
            {
                entity.Property(e => e.Date).IsUnicode(false);

                entity.Property(e => e.DateOfEvents).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);
            });

            modelBuilder.Entity<AppointmentTbl>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue((short)0);

                entity.Property(e => e.IsViewed).HasDefaultValue(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.Property(e => e.Time).IsUnicode(false);

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.AppointmentTbls)
                    .HasForeignKey(d => d.Owner_ID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppointmentTbl_OwnerTbl");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.AppointmentTbls)
                    .HasForeignKey(d => d.Patient_ID)
                    .HasConstraintName("FK_AppointmentTbl_PatientTbl");
            });

            modelBuilder.Entity<ConsultationTbl>(entity =>
            {
                entity.Property(e => e.ClinicalSign)
                    .IsUnicode(false)
                    .HasDefaultValue("N/A");

                entity.Property(e => e.Diagnosis)
                    .IsUnicode(false)
                    .HasDefaultValue("N/A");

                entity.Property(e => e.HeartRate).IsUnicode(false);

                entity.Property(e => e.RepiratoryRate).IsUnicode(false);

                entity.Property(e => e.Temparature).IsUnicode(false);

                entity.Property(e => e.Weight).IsUnicode(false);

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.ConsultationTbls)
                    .HasForeignKey(d => d.Patient_ID)
                    .HasConstraintName("FK_ConsultationTbl_PatientTbl");
            });

            modelBuilder.Entity<EventsTbl>(entity =>
            {
                entity.Property(e => e.Dates).IsUnicode(false);

                entity.Property(e => e.TItle).IsUnicode(false);
            });

            modelBuilder.Entity<LaboratoryTbl>(entity =>
            {
                entity.Property(e => e.LaboratoryName).IsUnicode(false);

                entity.Property(e => e.Status).IsFixedLength();

                entity.Property(e => e.LabFileName).IsUnicode(false);

                entity.HasOne(d => d.Consultation)
                    .WithMany(p => p.LaboratoryTbls)
                    .HasForeignKey(d => d.Consultation_ID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LaboratoryTbl_ConsultationTbl");
            });

            modelBuilder.Entity<OwnerTbl>(entity =>
            {
                entity.HasKey(e => e.Owner_ID)
                    .HasName("PK_OwnerTbl_1");

                entity.Property(e => e.Baranggay).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Firstname).IsUnicode(false);

                entity.Property(e => e.Lastname).IsUnicode(false);

                entity.Property(e => e.Midname).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.Province).IsUnicode(false);

                entity.Property(e => e.Sex).IsUnicode(false);

                entity.Property(e => e.Street).IsFixedLength();

                entity.Property(e => e.SuffixName).IsFixedLength();
            });

            modelBuilder.Entity<PatientTbl>(entity =>
            {
                entity.Property(e => e.Breed).IsUnicode(false);

                entity.Property(e => e.ColorMarking).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.FileName).IsUnicode(false);

                entity.Property(e => e.PatientName).IsUnicode(false);

                entity.Property(e => e.Sex).IsUnicode(false);

                entity.Property(e => e.Species).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

				entity.Property(e => e.IsDeleted).HasDefaultValue((short)0);

				entity.HasOne(d => d.Owner)
                    .WithMany(p => p.PatientTbls)
                    .HasForeignKey(d => d.Owner_ID)
                    .HasConstraintName("FK_PatientTbl_OwnerTbl");
            });

            modelBuilder.Entity<PrescriptionDetailTbl>(entity =>
            {
                entity.Property(e => e.Dosage).IsUnicode(false);

                entity.Property(e => e.Frequency).IsUnicode(false);

                entity.Property(e => e.Medicine).IsUnicode(false);

                entity.HasOne(d => d.Prescription)
                    .WithMany(p => p.PrescriptionDetailTbls)
                    .HasForeignKey(d => d.Prescription_ID)
                    .HasConstraintName("FK_PrescriptionDetailTbl_PrescriptionTbl1");
            });

            modelBuilder.Entity<PrescriptionTbl>(entity =>
            {
                entity.Property(e => e.Disease).IsUnicode(false);

                entity.HasOne(d => d.AdminInfo)
                    .WithMany(p => p.PrescriptionTbls)
                    .HasForeignKey(d => d.AdminInfo_ID)
                    .HasConstraintName("FK_PrescriptionTbl_AdminInfoTbl");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PrescriptionTbls)
                    .HasForeignKey(d => d.Patient_ID)
                    .HasConstraintName("FK_PrescriptionTbl_PatientTbl");
            });

            modelBuilder.Entity<ServiceTbl>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue((short)0);

                entity.Property(e => e.ServiceName).IsUnicode(false);
            });

            modelBuilder.Entity<TreatmentTbl>(entity =>
            {
                entity.HasKey(e => e.Treatment_ID)
                    .HasName("PK_TreamentPlanTbl");

                entity.Property(e => e.Notes).IsUnicode(false);

                entity.Property(e => e.TreatmentType).IsUnicode(false);

                entity.HasOne(d => d.Consultation)
                    .WithMany(p => p.TreatmentTbls)
                    .HasForeignKey(d => d.Consultation_ID)
                    .HasConstraintName("FK_TreamentPlanTbl_ConsultationTbl");
            });

            modelBuilder.Entity<UserTbl>(entity =>
            {
                entity.HasKey(e => e.User_ID)
                    .HasName("PK_UserTbl_1");

                entity.Property(e => e.ConfirmPassword).IsUnicode(false);

                entity.Property(e => e.IsActivated).HasDefaultValue((short)0);

                entity.Property(e => e.IsDeleted).HasDefaultValue((short)0);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.Roles)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.UserTbls)
                    .HasForeignKey(d => d.Owner_ID)
                    .HasConstraintName("FK_UserTbl_OwnerTbl");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
