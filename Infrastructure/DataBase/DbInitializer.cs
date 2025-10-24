using Back.Domain.Entity;

namespace Back.Infrastructure.DataBase
{
    public static class DbInitializer
    {
        // ⚙️ вызывается при старте приложения
        public static async Task EnsureCreatedAndSeedAsync(ApplicationDbContext context, bool seedEnabled)
        {
            // 1️⃣ создаём базу данных, если она не существует
            await context.Database.EnsureCreatedAsync();

            // 2️⃣ если сидинг выключен — выходим
            if (!seedEnabled)
                return;

            // 3️⃣ проверяем, есть ли пользователи
            if (context.Users.Any())
                return;

            var random = new Random();

            // списки для генерации данных
            string[] cities = { "Москва", "Ростов-на-Дону", "Казань", "Екатеринбург", "Санкт-Петербург" };
            string[] skills = { "C#", "Python", "SQL", "Figma", "ML", "Data Science", "React", "Blender", "3D Design", "Unity" };
            string[] interests = { "Музыка", "Игры", "Фильмы", "Искусственный интеллект", "Путешествия", "Спорт" };
            string[] requests = { "Найти команду", "Участвовать в хакатоне", "Создать стартап", "Получить опыт", "Прокачать скиллы" };
            string[] hobbies = { "Бег", "Рисование", "Программирование", "Фотография", "Йога", "Настольные игры" };
            string[] genders = { "М", "Ж" };

            var users = new List<User>();

            for (int i = 1; i <= 20; i++)
            {
                string name = $"User{i}";
                string surName = $"Testov{i}";
                string father = "Demo";
                string city = cities[random.Next(cities.Length)];
                string gender = genders[random.Next(genders.Length)];
                int age = random.Next(18, 35);

                var user = new User(
                    login: $"user{i}",
                    hashPassword: "hash123",
                    photoHash: $"photo{i}",
                    name: name,
                    surName: surName,
                    fatherName: father,
                    age: age,
                    gender: gender,
                    city: city,
                    contact: $"user{i}@mail.com"
                );

                // добавляем 2–3 случайных навыка
                foreach (var s in skills.OrderBy(x => random.Next()).Take(3))
                    user.AddSkill(s);

                // добавляем 1–2 интереса
                foreach (var inter in interests.OrderBy(x => random.Next()).Take(2))
                    user.AddInterest(inter);

                // ⚙️ UserGoal теперь заменяет Request — добавляем цели
                foreach (var g in requests.OrderBy(x => random.Next()).Take(1))
                    user.AddRequests("Тестовое название",g);

                // добавляем хобби
                foreach (var h in hobbies.OrderBy(x => random.Next()).Take(2))
                    user.AddHobby(h);

                users.Add(user);
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
