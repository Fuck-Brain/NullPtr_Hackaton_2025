using Back.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.DataBase
{
    public static class DbInitializer
    {
        public static async Task EnsureCreatedAndSeedAsync(ApplicationDbContext context, bool seedEnabled)
        {
            await context.Database.EnsureCreatedAsync();

            if (!seedEnabled || context.Users.Any())
                return;

            var random = new Random();

            string[] cities = { "Москва", "Санкт-Петербург", "Казань", "Екатеринбург", "Новосибирск", "Ростов-на-Дону", "Воронеж", "Самара", "Уфа", "Пермь" };
            string[] genders = { "М", "Ж" };

            string[] domains = { "IT", "Медицина", "Образование", "Бизнес", "Строительство", "Искусство", "Музыка", "Маркетинг", "Наука", "Спорт" };

            string[] skillsByDomain = {
                "C#, Python, SQL, DevOps, Аналитика данных",
                "Диагностика, Хирургия, Педиатрия, Фармакология",
                "Преподавание, Психология, Методология, Педагогика",
                "Менеджмент, Продажи, Финансы, Коммуникации",
                "Архитектура, Монтаж, Электрика, Проектирование",
                "Живопись, Иллюстрация, Дизайн, Скульптура",
                "Гитара, Вокал, Саунд-дизайн, Композиция",
                "SEO, SMM, Реклама, Аналитика",
                "Физика, Биология, Исследования, Статистика",
                "Фитнес, Диетология, Тренировки, Реабилитация"
            };

            string[] hobbies = { "Бег", "Рисование", "Пение", "Путешествия", "Йога", "Настольные игры", "Фотография", "Чтение", "Кулинария", "Танцы" };
            string[] interests = { "Искусство", "Наука", "Психология", "Музыка", "Технологии", "Образование", "Бизнес", "Природа", "ЗОЖ", "Путешествия" };

            string[] labels = {
                "человек, который ищёт друзей или единомышленников",
                "человек, который ищет сотрудника или исполнителя",
                "человек, который оказывает или продаёт услуги",
                "человек, который ищет работу или проект",
                "человек, который хочет получить услугу"
            };

            var users = new List<User>();

            // 1️⃣ Создаём 100 пользователей
            for (int i = 1; i <= 20; i++)
            {
                int domainIndex = random.Next(domains.Length);
                string domain = domains[domainIndex];
                string[] skillPool = skillsByDomain[domainIndex].Split(", ");
                string name = $"User{i}";
                string surName = $"Testov{i}";
                string fatherName = "Demo";
                string city = cities[random.Next(cities.Length)];
                string gender = genders[random.Next(genders.Length)];
                int age = random.Next(18, 60);

                var user = new User(
                    login: $"user{i}",
                    hashPassword: "hash123",
                    photoHash: String.Empty,
                    name: name,
                    surName: surName,
                    fatherName: fatherName,
                    age: age,
                    gender: gender,
                    city: city,
                    contact: $"user{i}@mail.com"
                );

                foreach (var skill in skillPool.OrderBy(x => random.Next()).Take(random.Next(2, 4)))
                    user.AddSkill(skill);

                foreach (var h in hobbies.OrderBy(x => random.Next()).Take(random.Next(1, 3)))
                    user.AddHobby(h);

                foreach (var inter in interests.OrderBy(x => random.Next()).Take(random.Next(1, 3)))
                    user.AddInterest(inter);

                users.Add(user);
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // 2️⃣ Добавляем Requests
            var allUsers = await context.Users.ToListAsync();

            foreach (var user in allUsers)
            {
                var label = labels[random.Next(labels.Length)];

                // выбираем случайного "адресата", не совпадающего с текущим пользователем
                var targetUser = allUsers[random.Next(allUsers.Count)];
                if (targetUser.Id == user.Id)
                    continue;

                string text = label switch
                {
                    "человек, который ищёт друзей или единомышленников" =>
                        $"Привет! Я {user.Name} из {user.City}, хочу найти друзей или людей, которые тоже увлекаются {string.Join(", ", user.Hobbies.Select(h => h.HobbyName))}. Общение и совместные проекты приветствуются!",

                    "человек, который ищет сотрудника или исполнителя" =>
                        $"Ищу исполнителя для совместной работы в сфере {string.Join(", ", user.Skills.Select(s => s.SkillName))}. Буду рад сотрудничеству!",

                    "человек, который оказывает или продаёт услуги" =>
                        $"Предлагаю свои услуги в области {string.Join(", ", user.Skills.Select(s => s.SkillName))}. Опыт — {random.Next(2, 10)} лет. Готов к новым клиентам!",

                    "человек, который ищет работу или проект" =>
                        $"Я ищу интересный проект или работу в области {string.Join(", ", user.Skills.Select(s => s.SkillName))}. Открыт к предложениям!",

                    "человек, который хочет получить услугу" =>
                        $"Хочу найти специалиста, который поможет мне в {string.Join(", ", user.Skills.Select(s => s.SkillName.Take(1)))} или близкой сфере. Буду благодарен за советы!",

                    _ => "Открыт к общению и новым возможностям!"
                };

                var request = new Request(
                     targetUser.Id,   //  адресуем запрос другому
                     $"Запрос от {user.Name}",
                     text
                );

                // добавляем метку
                request.SetLabel(label);

                await context.Requests.AddAsync(request);
            }

            await context.SaveChangesAsync();
        }
    }
}
