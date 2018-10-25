using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class WorldContext : DbContext, ISeed<WorldContext>
    {
        public WorldContext()
        {
        }

        public WorldContext(DbContextOptions<WorldContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Entity> Entity { get; set; }
        public virtual DbSet<EntityVendor> EntityVendor { get; set; }
        public virtual DbSet<EntityVendorCategory> EntityVendorCategory { get; set; }
        public virtual DbSet<EntityVendorItem> EntityVendorItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(DatabaseManager.Config, DatabaseType.World);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>(entity =>
            {
                entity.ToTable("entity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Creature)
                    .HasColumnName("creature")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DisplayInfo)
                    .HasColumnName("displayInfo")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Faction1)
                    .HasColumnName("faction1")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Faction2)
                    .HasColumnName("faction2")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.OutfitInfo)
                    .HasColumnName("outfitInfo")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Rx)
                    .HasColumnName("rx")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Ry)
                    .HasColumnName("ry")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Rz)
                    .HasColumnName("rz")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.World)
                    .HasColumnName("world")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.X)
                    .HasColumnName("x")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Y)
                    .HasColumnName("y")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Z)
                    .HasColumnName("z")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<EntityVendor>(entity =>
            {
                entity.ToTable("entity_vendor");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BuyPriceMultiplier)
                    .HasColumnName("buyPriceMultiplier")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.SellPriceMultiplier)
                    .HasColumnName("sellPriceMultiplier")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.EntityVendor)
                    .HasForeignKey<EntityVendor>(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorCategory>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index });

                entity.ToTable("entity_vendor_category");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LocalisedTextId)
                    .HasColumnName("localisedTextId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityVendorCategory)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_category_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index });

                entity.ToTable("entity_vendor_item");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CategoryIndex)
                    .HasColumnName("categoryIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityVendorItem)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_item_id__entity_id");
            });
        }
        // Everything below this line likely belongs somewhere else.

        private Entity MakeEntity(uint id, byte type, uint creature, ushort world, double x, double y, double z, double rx,
            double ry, double rz, uint displayInfo, ushort outfitInfo, ushort faction1, ushort faction2)
        {
            return new Entity()
            {
                Id = id,
                Type = type,
                Creature = creature,
                World = world,
                X = (float)x,
                Y = (float)y,
                Z = (float)z,
                Rx = (float)rx,
                Ry = (float)ry,
                Rz = (float)rz,
                DisplayInfo = displayInfo,
                OutfitInfo = outfitInfo,
                Faction1 = faction1,
                Faction2 = faction2
            };
        }

        private EntityVendor MakeEntityVendor(uint id, double buyPriceMultiplier, double sellPriceMultiplier)
        {
            return new EntityVendor()
            {
                Id = id,
                BuyPriceMultiplier = (float)buyPriceMultiplier,
                SellPriceMultiplier = (float)sellPriceMultiplier
            };
        }

        private EntityVendorCategory MakeEntityVendorCategory(uint id, uint index, uint localisedTextId)
        {
            return new EntityVendorCategory()
            {
                Id = id,
                Index = index,
                LocalisedTextId = localisedTextId
            };
        }

        private EntityVendorItem MakeEntityVendorItem(uint id, uint index, uint categoryIndex, uint itemId)
        {
            return new EntityVendorItem()
            {
                Id = id,
                Index = index,
                CategoryIndex = categoryIndex,
                ItemId = itemId
            };
        }

        public void Seed(WorldContext context)
        {
            // This data should come from JSON or CSV, but for now, this will do.
            var entitySeedData = new[]
            {
                MakeEntity(262277, 0, 73334, 870, -7700.06, -940.929, -667.332, -2.56057, 0, 0, 21339, 0, 255, 255),
                MakeEntity(262278, 0, 36464, 870, -7697.02, -940.711, -650.236, -1.88312, 0, 0, 35188, 9154, 170, 170),
                MakeEntity(262289, 0, 24187, 870, -7664.51, -942.745, -672.721, 0.839049, 0, 0, 25202, 9079, 170, 170),
                MakeEntity(262291, 0, 24158, 870, -7666.19, -942.697, -678.087, -3.02895, 0, 0, 25194, 9076, 170, 170),
                MakeEntity(262301, 0, 33538, 870, -7623.78, -950.75, -668.586, -3.14159, 0, 0, 28247, 0, 174, 174),
                MakeEntity(262302, 0, 33538, 870, -7568.85, -963.021, -680.979, -3.14159, 0, 0, 28247, 0, 174, 174),
                MakeEntity(262303, 0, 33538, 870, -7592.03, -955.337, -658.009, -3.14159, 0, 0, 28247, 0, 174, 174),
                MakeEntity(262325, 0, 24244, 870, -7513.56, -976.798, -804.925, -1.08664, 0, 0, 26049, 0, 242, 242),
                MakeEntity(262474, 0, 24054, 870, -7583.89, -954.865, -626.049, -1.56308, 0, 0, 26120, 0, 550, 550),
                MakeEntity(262476, 0, 24054, 870, -7563.61, -961.314, -719.553, -3.14159, 0, 0, 26120, 0, 550, 550),
                MakeEntity(262479, 0, 24054, 870, -7553.58, -975.297, -868.729, -2.25485, 0, 0, 26120, 0, 550, 550),
                MakeEntity(262812, 0, 37219, 870, -7732.75, -861.079, -555.471, -1.09296, -0.350791, 0.175443, 21782, 0, 219, 219),
                MakeEntity(262849, 0, 25740, 870, -7682.52, -941.223, -684.335, 2.95349, 0, 0, 28210, 9005, 170, 170),
                MakeEntity(263020, 0, 25685, 870, -7841.17, -949.242, -272.99, 1.41686, 0, 0, 26688, 0, 219, 219),
                MakeEntity(263029, 0, 24491, 870, -7663.98, -942.842, -683.979, 2.52204, 0, 0, 26070, 9777, 170, 170),
                MakeEntity(263034, 0, 51870, 870, -7704.48, -940.841, -650.06, 3.09754, 0, 0, 26575, 9004, 170, 170),
                MakeEntity(263035, 0, 51870, 870, -7652.15, -943.4, -682.637, 2.2836, 0, 0, 28208, 9004, 170, 170),
                MakeEntity(263040, 0, 32818, 870, -7722.6, -941.677, -733.102, 0.011311, -0.0795956, -0.0235451, 26314, 9058, 170, 170),
                MakeEntity(263042, 0, 32817, 870, -7721.13, -941.882, -735.195, 2.29117, 0, 0, 26072, 9004, 170, 170),
                MakeEntity(263084, 0, 24058, 870, -7571.77, -958.999, -645.823, -1.34628, 0, 0, 21664, 0, 413, 413),
                MakeEntity(263095, 0, 32851, 870, -7701.68, -933.852, -701.023, -0.762341, 0, 0, 25926, 9203, 170, 170),
                MakeEntity(263096, 0, 32822, 870, -7741.98, -941.703, -734.395, 2.46608, 0, 0, 25940, 9057, 170, 170),
                MakeEntity(263097, 0, 32825, 870, -7704.88, -942.503, -733.927, -1.3258, 0, 0, 23012, 0, 219, 219),
                MakeEntity(263098, 0, 32825, 870, -7700.98, -941.346, -758.05, -0.357624, 0, 0, 23012, 0, 219, 219),
                MakeEntity(263099, 0, 32823, 870, -7705.71, -942.119, -746.886, -1.64484, 0, 0, 31431, 9057, 170, 170),
                MakeEntity(263100, 0, 32822, 870, -7738.84, -941.376, -750.362, 0.129376, 0, 0, 25916, 9057, 170, 170),
                MakeEntity(263101, 0, 32825, 870, -7713.25, -940.666, -752.904, 0.844349, 0, 0, 23012, 0, 219, 219),
                MakeEntity(263102, 0, 32781, 870, -7691.29, -938.236, -614.972, -2.46045, 0, 0, 26571, 9004, 170, 170),
                MakeEntity(263103, 0, 32781, 870, -7655.06, -943.469, -681.412, -1.52351, 0, 0, 27533, 9003, 170, 170),
                MakeEntity(263104, 0, 32781, 870, -7646.81, -942.61, -638.554, -3.14159, 0, 0, 27536, 9004, 170, 170),
                MakeEntity(263105, 0, 32780, 870, -7700.34, -940.507, -648.308, 2.74308, 0, 0, 26072, 9004, 170, 170),
                MakeEntity(263108, 0, 25592, 870, -7684.32, -941.307, -681.645, -0.581855, 0.0716395, -0.0272887, 31403, 9058, 170, 170),
                MakeEntity(263263, 0, 25378, 870, -7687.77, -864.276, -600.977, -2.94363, -0.000000000926764, -0.0000000000920319, 26316, 9057, 170, 170),
                MakeEntity(264030, 0, 11254, 870, -7716.56, -935.766, -602.192, 0, 0, 0, 22968, 0, 219, 219)
            };

            var entityVendorSeed = new[]
            {
                MakeEntityVendor(263029, 1, 1)
            };


            var entityVendorCategorySeed = new[]
            {
                MakeEntityVendorCategory(263029, 1, 712282),
                MakeEntityVendorCategory(263029, 2, 712302),
                MakeEntityVendorCategory(263029, 3, 712312),
                MakeEntityVendorCategory(263029, 4, 712322)
            };

            var entityVendorItemSeed = new[]
            {
                MakeEntityVendorItem(263029, 1, 1, 13179),
                MakeEntityVendorItem(263029, 2, 1, 13176),
                MakeEntityVendorItem(263029, 3, 1, 13194),
                MakeEntityVendorItem(263029, 4, 1, 13188),
                MakeEntityVendorItem(263029, 5, 1, 13191),
                MakeEntityVendorItem(263029, 6, 1, 13185),
                MakeEntityVendorItem(263029, 7, 2, 13148),
                MakeEntityVendorItem(263029, 8, 2, 13151),
                MakeEntityVendorItem(263029, 9, 2, 13154),
                MakeEntityVendorItem(263029, 10, 2, 83720),
                MakeEntityVendorItem(263029, 11, 2, 13157),
                MakeEntityVendorItem(263029, 12, 2, 83721),
                MakeEntityVendorItem(263029, 13, 3, 13160),
                MakeEntityVendorItem(263029, 14, 3, 13163),
                MakeEntityVendorItem(263029, 15, 3, 13166),
                MakeEntityVendorItem(263029, 16, 3, 83722),
                MakeEntityVendorItem(263029, 17, 3, 13169),
                MakeEntityVendorItem(263029, 18, 3, 83723),
                MakeEntityVendorItem(263029, 19, 4, 27873),
                MakeEntityVendorItem(263029, 20, 4, 27875),
                MakeEntityVendorItem(263029, 21, 4, 27876),
                MakeEntityVendorItem(263029, 22, 4, 83724),
                MakeEntityVendorItem(263029, 23, 4, 27874),
                MakeEntityVendorItem(263029, 24, 4, 83725)
            };

            using (var transaction = context.Database.BeginTransaction())
            {
                SeedItems(context, entitySeedData, i => new object[] { i.Id });
                SeedItems(context, entityVendorSeed, i => new object[] { i.Id });
                SeedItems(context, entityVendorCategorySeed, i => new object[] { i.Id, i.Index });
                SeedItems(context, entityVendorItemSeed, i => new object[] { i.Id, i.Index });
                transaction.Commit();
            }
        }

        private static void SeedItems<TItem>(WorldContext context, IEnumerable<TItem> items,
            Func<TItem, object[]> keySelector) where TItem : class
        {
            foreach (var item in items)
            {
                var existing = context.Set<TItem>().Find(keySelector(item));
                if (existing == null)
                {
                    context.Set<TItem>().Add(item);
                    continue;
                }
                context.Entry(existing).CurrentValues.SetValues(item);
            }
            context.SaveChanges();
        }
    }
}
