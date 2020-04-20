using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesAPi.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Design;
using Npgsql.NameTranslation;

namespace MoviesAPi.PostgreSqlProvider
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);
            modelBuilder.ForNpgsqlUseSequenceHiLo();

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<long>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("UsersClaim");
            modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("RolesClaim");

            modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("UsersLogin");
            modelBuilder.Entity<IdentityUserRole<long>>().ToTable("UsersRole");
            modelBuilder.Entity<IdentityUserToken<long>>().ToTable("UsersToken");

            modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.GenreId, x.MovieId });
            modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.PersonId, x.MovieId });

            ApplySnakeCaseNames(modelBuilder);

            SeedData(modelBuilder);


        }

        private void ApplySnakeCaseNames(ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // modify column names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(mapper.TranslateMemberName(property.GetColumnName()));
                }

                // modify table name
                entity.SetTableName(mapper.TranslateMemberName(entity.GetTableName()));

                // move asp_net tables into schema 'identity'
                if (entity.GetTableName().StartsWith("asp_net_"))
                {
                    entity.SetTableName(entity.GetTableName().Replace("asp_net_", string.Empty));
                    entity.SetSchema("identity");
                }
            }
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var adventure = new Genre() { Id = 4, Name = "Adventure" };
            var animation = new Genre() { Id = 5, Name = "Animation" };
            var drama = new Genre() { Id = 6, Name = "Drama" };
            var romance = new Genre() { Id = 7, Name = "Romance" };

            modelBuilder.Entity<Genre>()
                .HasData(new List<Genre>
                {
                    adventure, animation, drama, romance
                });

            var jimCarrey = new Person() { Id = 5, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 01, 17) };
            var robertDowney = new Person() { Id = 6, Name = "Robert Downey Jr.", DateOfBirth = new DateTime(1965, 4, 4) };
            var chrisEvans = new Person() { Id = 7, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Person>()
                .HasData(new List<Person>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Movie()
            {
                Id = 2,
                Title = "Avengers: Endgame",
                InTheaters = true,
                ReleaseDate = new DateTime(2019, 04, 26)
            };

            var iw = new Movie()
            {
                Id = 3,
                Title = "Avengers: Infinity Wars",
                InTheaters = false,
                ReleaseDate = new DateTime(2019, 04, 26)
            };

            var sonic = new Movie()
            {
                Id = 4,
                Title = "Sonic the Hedgehog",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 28)
            };
            var emma = new Movie()
            {
                Id = 5,
                Title = "Emma",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };
            var greed = new Movie()
            {
                Id = 6,
                Title = "Greed",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };

            modelBuilder.Entity<Movie>()
                .HasData(new List<Movie>
                {
                    endgame, iw, sonic, emma, greed
                });

            modelBuilder.Entity<MoviesGenres>().HasData(
                new List<MoviesGenres>()
                {
                    new MoviesGenres(){MovieId = endgame.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = endgame.Id, GenreId = adventure.Id},
                    new MoviesGenres(){MovieId = iw.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = iw.Id, GenreId = adventure.Id},
                    new MoviesGenres(){MovieId = sonic.Id, GenreId = adventure.Id},
                    new MoviesGenres(){MovieId = emma.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = emma.Id, GenreId = romance.Id},
                    new MoviesGenres(){MovieId = greed.Id, GenreId = drama.Id},
                    new MoviesGenres(){MovieId = greed.Id, GenreId = romance.Id},
                });

            modelBuilder.Entity<MoviesActors>().HasData(
                new List<MoviesActors>()
                {
                    new MoviesActors(){MovieId = endgame.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = endgame.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = iw.Id, PersonId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = iw.Id, PersonId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = sonic.Id, PersonId = jimCarrey.Id, Character = "Dr. Ivo Robotnik", Order = 1}
                });
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
    }
}