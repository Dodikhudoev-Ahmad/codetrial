using CodeTrail.Application.Auth;
using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Seeding;

public static class CourseSeeder
{
    // Test credential for the seeded first administrator, documented in the README
    // once launch instructions are written. Anyone with repo access can derive it
    // from this constant, so it must never be reused for a real account.
    public const string AdminEmail = "admin@codetrail.local";
    public const string AdminPassword = "Admin123!";

    public static async Task SeedAsync(CodeTrailDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Courses.AnyAsync())
        {
            return;
        }

        var author = new User
        {
            Email = AdminEmail,
            PasswordHash = passwordHasher.Hash(AdminPassword),
            DisplayName = "CodeTrail Team",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(author);
        context.Courses.AddRange(BuildCSharpCourse(author), BuildSqlCourse(author));

        await context.SaveChangesAsync();
    }

    private static Course BuildCSharpCourse(User author)
    {
        return NewCourse(
            title: "Основы C#",
            slug: "csharp-basics",
            description: "Базовый курс для тех, кто делает первые шаги в программировании на C#. " +
                          "Разберём переменные, типы данных, условные операторы, циклы, массивы и методы " +
                          "— с практическими примерами и тестами после каждой темы.",
            level: CourseLevel.Beginner,
            language: "C#",
            author: author,
            lessons: new[]
            {
                NewLesson(1, "Переменные и типы данных",
                    """
                    # Переменные и типы данных

                    В C# каждая переменная имеет **тип**, который определяет, какие значения она может хранить и какие операции над ней допустимы.

                    ## Основные типы

                    - `int` — целые числа (например, `42`, `-7`)
                    - `double` — числа с плавающей точкой (например, `3.14`)
                    - `bool` — логическое значение: `true` или `false`
                    - `string` — текст, например `"Hello, world!"`
                    - `char` — один символ, например `'A'`

                    ## Объявление переменной

                    ```csharp
                    int age = 25;
                    string name = "Anna";
                    bool isStudent = true;
                    ```

                    Слева от имени переменной указывается тип, справа — значение. C# — язык со строгой типизацией: переменной типа `int` нельзя присвоить строку без явного преобразования.

                    ## var — вывод типа

                    Компилятор может сам определить тип по значению справа:

                    ```csharp
                    var count = 10; // компилятор понимает, что это int
                    ```

                    `var` — это не «любой тип», а просто удобная запись: тип всё равно фиксируется на этапе компиляции.
                    """,
                    xpReward: 10,
                    SingleChoice(1,
                        "Какой тип данных используется для хранения текста в C#?",
                        "string хранит последовательность символов (текст). char хранит только один символ, поэтому для текста не подходит.",
                        null,
                        ("int", false), ("string", true), ("bool", false), ("char", false)),
                    ShortAnswer(2,
                        "Какое ключевое слово позволяет компилятору самостоятельно определить тип переменной по присваиваемому значению?",
                        "Ключевое слово var указывает компилятору вывести тип переменной из значения справа от знака равенства.",
                        expectedAnswer: "var"),
                    MultiChoice(3,
                        "Какие из перечисленных типов являются встроенными числовыми типами C#?",
                        "int и double — числовые типы (целые и с плавающей точкой). string — текст, bool — логическое значение.",
                        null,
                        ("int", true), ("double", true), ("string", false), ("bool", false))),

                NewLesson(2, "Условные операторы",
                    """
                    # Условные операторы

                    Условные операторы позволяют выполнять разные блоки кода в зависимости от условия.

                    ## if / else

                    ```csharp
                    int score = 65;

                    if (score >= 70)
                    {
                        Console.WriteLine("Зачёт");
                    }
                    else
                    {
                        Console.WriteLine("Не зачёт");
                    }
                    ```

                    ## else if

                    Можно проверить несколько условий подряд:

                    ```csharp
                    if (score >= 90)
                    {
                        Console.WriteLine("Отлично");
                    }
                    else if (score >= 70)
                    {
                        Console.WriteLine("Хорошо");
                    }
                    else
                    {
                        Console.WriteLine("Нужно повторить материал");
                    }
                    ```

                    ## Тернарный оператор

                    Короткая запись для простого if/else, возвращающая значение:

                    ```csharp
                    string result = score >= 70 ? "Зачёт" : "Не зачёт";
                    ```

                    ## switch

                    Удобен, когда вариантов много:

                    ```csharp
                    switch (dayOfWeek)
                    {
                        case 1: Console.WriteLine("Понедельник"); break;
                        case 7: Console.WriteLine("Воскресенье"); break;
                        default: Console.WriteLine("Будний или выходной день"); break;
                    }
                    ```
                    """,
                    xpReward: 10,
                    SingleChoice(1,
                        "Чему будет равно значение result в следующем коде?",
                        "x = 5, что не больше 10, поэтому тернарный оператор возвращает вторую ветку — \"меньше или равно\".",
                        "int x = 5;\nstring result = x > 10 ? \"больше\" : \"меньше или равно\";",
                        ("больше", false), ("меньше или равно", true), ("ошибка компиляции", false)),
                    MultiChoice(2,
                        "Какие из операторов используются в C# для условного выполнения кода?",
                        "if и switch — условные конструкции. for и while — циклы, а не условные операторы.",
                        null,
                        ("if", true), ("switch", true), ("for", false), ("while", false))),

                NewLesson(3, "Циклы",
                    """
                    # Циклы

                    Циклы позволяют повторять блок кода несколько раз без дублирования кода.

                    ## for

                    Используется, когда известно количество повторений:

                    ```csharp
                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine(i);
                    }
                    ```

                    ## while

                    Выполняется, пока условие истинно:

                    ```csharp
                    int i = 0;
                    while (i < 5)
                    {
                        Console.WriteLine(i);
                        i++;
                    }
                    ```

                    ## do-while

                    Тело цикла выполнится хотя бы один раз, даже если условие изначально ложно:

                    ```csharp
                    int i = 0;
                    do
                    {
                        Console.WriteLine(i);
                        i++;
                    } while (i < 5);
                    ```

                    ## foreach

                    Удобен для перебора коллекций:

                    ```csharp
                    int[] numbers = { 1, 2, 3 };
                    foreach (var n in numbers)
                    {
                        Console.WriteLine(n);
                    }
                    ```

                    ## break и continue

                    `break` досрочно прерывает цикл, `continue` пропускает текущую итерацию и переходит к следующей.
                    """,
                    xpReward: 10,
                    SingleChoice(1,
                        "Сколько раз выведется число на экран в следующем коде?",
                        "Цикл выполняется при i = 0, 1, 2 — три итерации, так как условие i < 3 становится ложным при i = 3.",
                        "for (int i = 0; i < 3; i++)\n{\n    Console.WriteLine(i);\n}",
                        ("2", false), ("3", true), ("4", false)),
                    ShortAnswer(2,
                        "Какое ключевое слово используется для досрочного прерывания цикла в C#?",
                        "break немедленно завершает выполнение цикла.",
                        expectedAnswer: "break"),
                    MultiChoice(3,
                        "Какие из перечисленных конструкций являются циклами в C#?",
                        "for, while и foreach — циклы. if — условный оператор, а не цикл.",
                        null,
                        ("for", true), ("while", true), ("foreach", true), ("if", false))),

                NewLesson(4, "Массивы и списки",
                    """
                    # Массивы и списки

                    ## Массивы

                    Массив хранит фиксированное количество элементов одного типа:

                    ```csharp
                    int[] numbers = { 1, 2, 3, 4, 5 };
                    Console.WriteLine(numbers[0]); // 1
                    Console.WriteLine(numbers.Length); // 5
                    ```

                    Размер массива нельзя изменить после создания.

                    ## List<T>

                    `List<T>` — динамическая коллекция, которая может расти и уменьшаться:

                    ```csharp
                    List<string> names = new List<string>();
                    names.Add("Anna");
                    names.Add("Boris");
                    names.Remove("Anna");
                    Console.WriteLine(names.Count); // 1
                    ```

                    ## Основные операции со списком

                    - `Add(item)` — добавить элемент
                    - `Remove(item)` — удалить элемент
                    - `Count` — количество элементов
                    - `Contains(item)` — проверка наличия элемента
                    - `list[i]` — доступ по индексу

                    ## Перебор

                    ```csharp
                    foreach (var name in names)
                    {
                        Console.WriteLine(name);
                    }
                    ```
                    """,
                    xpReward: 15,
                    SingleChoice(1,
                        "Чему равно значение numbers.Length?",
                        "Массив содержит три элемента: 10, 20 и 30, поэтому Length равен 3.",
                        "int[] numbers = { 10, 20, 30 };",
                        ("2", false), ("3", true), ("30", false)),
                    MultiChoice(2,
                        "Какие операции доступны у List<T>?",
                        "У List<T> есть Add, Remove и Count. Length — свойство массива, а не List<T> (у списка используется Count).",
                        null,
                        ("Add", true), ("Remove", true), ("Length", false), ("Count", true)),
                    ShortAnswer(3,
                        "Как называется метод для добавления элемента в конец List<T>?",
                        "Метод Add добавляет новый элемент в конец списка.",
                        expectedAnswer: "Add")),

                NewLesson(5, "Методы",
                    """
                    # Методы

                    Метод — именованный блок кода, который можно вызывать многократно.

                    ## Объявление метода

                    ```csharp
                    int Add(int a, int b)
                    {
                        return a + b;
                    }

                    int result = Add(2, 3); // 5
                    ```

                    Структура: тип возвращаемого значения, имя, параметры в скобках.

                    ## void — метод без возвращаемого значения

                    ```csharp
                    void Greet(string name)
                    {
                        Console.WriteLine($"Привет, {name}!");
                    }
                    ```

                    ## Параметры по умолчанию

                    ```csharp
                    void Greet(string name = "гость")
                    {
                        Console.WriteLine($"Привет, {name}!");
                    }

                    Greet(); // Привет, гость!
                    Greet("Anna"); // Привет, Anna!
                    ```

                    ## Перегрузка методов

                    Несколько методов с одним именем, но разными параметрами:

                    ```csharp
                    int Add(int a, int b) => a + b;
                    double Add(double a, double b) => a + b;
                    ```
                    """,
                    xpReward: 15,
                    SingleChoice(1,
                        "Что вернёт вызов Add(2, 3)?",
                        "Метод складывает параметры: 2 + 3 = 5.",
                        "int Add(int a, int b) => a + b;",
                        ("23", false), ("5", true), ("ошибка", false)),
                    SingleChoice(2,
                        "Какое ключевое слово используется для метода, который не возвращает значение?",
                        "void указывает, что метод не возвращает значение.",
                        null,
                        ("void", true), ("null", false), ("empty", false), ("none", false)),
                    MultiChoice(3,
                        "Что необходимо указать при объявлении метода в C#?",
                        "Метод обязательно имеет тип возврата, имя и скобки с параметрами (даже если они пустые). var не используется для объявления методов.",
                        null,
                        ("Тип возвращаемого значения", true), ("Имя метода", true),
                        ("Список параметров в скобках", true), ("Ключевое слово var", false))),
            });
    }

    private static Course BuildSqlCourse(User author)
    {
        return NewCourse(
            title: "SQL для разработчиков",
            slug: "sql-for-developers",
            description: "Практический курс по SQL: выборка данных, фильтрация, сортировка, агрегатные функции " +
                          "и объединение таблиц. Для тех, кто уже знаком с основами программирования и хочет " +
                          "уверенно работать с базами данных.",
            level: CourseLevel.Intermediate,
            language: "SQL",
            author: author,
            lessons: new[]
            {
                NewLesson(1, "Основы SELECT",
                    """
                    # Основы SELECT

                    `SELECT` — основная команда для получения данных из таблицы.

                    ## Выбор всех столбцов

                    ```sql
                    SELECT * FROM users;
                    ```

                    ## Выбор конкретных столбцов

                    ```sql
                    SELECT email, display_name FROM users;
                    ```

                    ## Псевдонимы (алиасы)

                    ```sql
                    SELECT display_name AS name FROM users;
                    ```

                    ## DISTINCT — уникальные значения

                    ```sql
                    SELECT DISTINCT language FROM courses;
                    ```

                    Возвращает только уникальные значения столбца `language`, без повторов.

                    ## LIMIT — ограничение количества строк

                    ```sql
                    SELECT * FROM courses LIMIT 5;
                    ```
                    """,
                    xpReward: 10,
                    SingleChoice(1,
                        "Какая команда используется для получения данных из таблицы?",
                        "SELECT — команда для чтения (выборки) данных из таблицы.",
                        null,
                        ("SELECT", true), ("INSERT", false), ("UPDATE", false), ("DELETE", false)),
                    ShortAnswer(2,
                        "Какое ключевое слово убирает повторяющиеся строки из результата запроса?",
                        "DISTINCT возвращает только уникальные строки, устраняя дубликаты.",
                        expectedAnswer: "DISTINCT"),
                    MultiChoice(3,
                        "Что делает запрос SELECT * FROM courses LIMIT 5;?",
                        "* означает все столбцы, LIMIT 5 ограничивает вывод пятью строками. Запрос не удаляет и не сортирует данные.",
                        null,
                        ("Возвращает все столбцы таблицы courses", true),
                        ("Ограничивает результат пятью строками", true),
                        ("Удаляет пять строк из таблицы", false),
                        ("Сортирует данные по возрастанию", false))),

                NewLesson(2, "Фильтрация и сортировка",
                    """
                    # WHERE и ORDER BY

                    ## WHERE — фильтрация строк

                    ```sql
                    SELECT * FROM courses WHERE level = 'Beginner';
                    ```

                    Условия можно комбинировать через `AND` и `OR`:

                    ```sql
                    SELECT * FROM courses
                    WHERE level = 'Beginner' AND is_published = true;
                    ```

                    ## Операторы сравнения

                    `=`, `<>` (не равно), `>`, `<`, `>=`, `<=`, `LIKE` (поиск по шаблону), `IN` (значение из списка).

                    ```sql
                    SELECT * FROM courses WHERE language IN ('C#', 'SQL');
                    SELECT * FROM courses WHERE title LIKE 'Основы%';
                    ```

                    ## ORDER BY — сортировка

                    ```sql
                    SELECT * FROM courses ORDER BY title ASC;
                    SELECT * FROM courses ORDER BY created_at DESC;
                    ```

                    `ASC` — по возрастанию (по умолчанию), `DESC` — по убыванию.
                    """,
                    xpReward: 10,
                    SingleChoice(1,
                        "Какое ключевое слово используется для фильтрации строк по условию?",
                        "WHERE задаёт условие, которому должны соответствовать возвращаемые строки.",
                        null,
                        ("WHERE", true), ("ORDER BY", false), ("GROUP BY", false), ("HAVING", false)),
                    SingleChoice(2,
                        "Как отсортировать результат по убыванию?",
                        "DESC сортирует результат по убыванию значений столбца.",
                        null,
                        ("ORDER BY column ASC", false), ("ORDER BY column DESC", true), ("SORT BY column DESC", false)),
                    MultiChoice(3,
                        "Какие операторы можно использовать в условии WHERE?",
                        "LIKE, IN и >= — операторы сравнения/фильтрации. ORDER — не оператор WHERE, а отдельное ключевое слово ORDER BY.",
                        null,
                        ("LIKE", true), ("IN", true), ("ORDER", false), (">=", true))),

                NewLesson(3, "Агрегатные функции и GROUP BY",
                    """
                    # Агрегатные функции и GROUP BY

                    Агрегатные функции вычисляют одно значение на основе множества строк.

                    ## Основные функции

                    - `COUNT(*)` — количество строк
                    - `SUM(column)` — сумма значений
                    - `AVG(column)` — среднее значение
                    - `MIN(column)` / `MAX(column)` — минимум и максимум

                    ```sql
                    SELECT COUNT(*) FROM enrollments;
                    SELECT AVG(xp_reward) FROM lessons;
                    ```

                    ## GROUP BY — группировка

                    Группирует строки по значению столбца и применяет агрегатную функцию к каждой группе:

                    ```sql
                    SELECT course_id, COUNT(*) AS lesson_count
                    FROM lessons
                    GROUP BY course_id;
                    ```

                    ## HAVING — фильтрация групп

                    `WHERE` фильтрует строки до группировки, `HAVING` — группы после группировки:

                    ```sql
                    SELECT course_id, COUNT(*) AS lesson_count
                    FROM lessons
                    GROUP BY course_id
                    HAVING COUNT(*) >= 4;
                    ```
                    """,
                    xpReward: 15,
                    SingleChoice(1,
                        "Какая функция возвращает количество строк в результате?",
                        "COUNT(*) подсчитывает количество строк.",
                        null,
                        ("COUNT", true), ("SUM", false), ("AVG", false), ("MAX", false)),
                    SingleChoice(2,
                        "В чём отличие HAVING от WHERE?",
                        "WHERE отбирает строки до группировки, HAVING — фильтрует уже сформированные группы, часто по результату агрегатной функции.",
                        null,
                        ("HAVING фильтрует группы после GROUP BY, WHERE — строки до группировки", true),
                        ("HAVING и WHERE полностью взаимозаменяемы", false),
                        ("WHERE применяется только к числовым столбцам", false)),
                    ShortAnswer(3,
                        "Какое ключевое слово группирует строки по значению столбца перед применением агрегатной функции?",
                        "GROUP BY объединяет строки с одинаковым значением столбца в группы.",
                        expectedAnswer: "GROUP BY")),

                NewLesson(4, "JOIN — объединение таблиц",
                    """
                    # JOIN

                    `JOIN` объединяет строки из двух и более таблиц на основе связанного столбца.

                    ## INNER JOIN

                    Возвращает только строки, для которых есть совпадение в обеих таблицах:

                    ```sql
                    SELECT e.id, u.display_name, c.title
                    FROM enrollments e
                    INNER JOIN users u ON e.user_id = u.id
                    INNER JOIN courses c ON e.course_id = c.id;
                    ```

                    ## LEFT JOIN

                    Возвращает все строки из левой таблицы, даже если совпадения в правой нет (в этом случае столбцы правой таблицы будут `NULL`):

                    ```sql
                    SELECT c.title, l.title AS lesson_title
                    FROM courses c
                    LEFT JOIN lessons l ON l.course_id = c.id;
                    ```

                    ## Ключ связи

                    Обычно JOIN выполняется по внешнему ключу (`foreign key`), например `course_id`, ссылающемуся на `id` таблицы `courses`.
                    """,
                    xpReward: 15,
                    SingleChoice(1,
                        "Какой JOIN вернёт все строки левой таблицы, даже без совпадений в правой?",
                        "LEFT JOIN сохраняет все строки левой таблицы; для строк без совпадения столбцы правой таблицы будут NULL.",
                        null,
                        ("INNER JOIN", false), ("LEFT JOIN", true), ("CROSS JOIN", false)),
                    MultiChoice(2,
                        "Что верно для INNER JOIN?",
                        "INNER JOIN возвращает только совпавшие строки, требует условия связи и может использоваться последовательно для объединения нескольких таблиц. Он не эквивалентен LEFT JOIN, который дополнительно сохраняет несовпавшие строки левой таблицы.",
                        null,
                        ("Возвращает только строки с совпадением в обеих таблицах", true),
                        ("Требует условие связи (обычно через ON)", true),
                        ("Эквивалентен LEFT JOIN", false),
                        ("Может объединять более двух таблиц", true)),
                    ShortAnswer(3,
                        "Каким ключевым словом задаётся условие связи между таблицами в JOIN?",
                        "Условие связи между таблицами в JOIN указывается после ключевого слова ON.",
                        expectedAnswer: "ON")),
            });
    }

    private static Course NewCourse(
        string title, string slug, string description, CourseLevel level, string language,
        User author, IEnumerable<Lesson> lessons)
    {
        var course = new Course
        {
            Title = title,
            Slug = slug,
            Description = description,
            Level = level,
            Language = language,
            IsPublished = true,
            Author = author
        };

        foreach (var lesson in lessons)
        {
            lesson.Course = course;
            course.Lessons.Add(lesson);
        }

        return course;
    }

    private static Lesson NewLesson(int order, string title, string theoryMarkdown, int xpReward, params Question[] questions)
    {
        var lesson = new Lesson
        {
            Order = order,
            Title = title,
            TheoryMarkdown = theoryMarkdown,
            XpReward = xpReward
        };

        foreach (var question in questions)
        {
            question.Lesson = lesson;
            lesson.Questions.Add(question);
        }

        return lesson;
    }

    private static Question SingleChoice(
        int order, string text, string explanation, string? codeSnippet, params (string Text, bool IsCorrect)[] options)
        => NewChoiceQuestion(QuestionType.SingleChoice, order, text, explanation, codeSnippet, options);

    private static Question MultiChoice(
        int order, string text, string explanation, string? codeSnippet, params (string Text, bool IsCorrect)[] options)
        => NewChoiceQuestion(QuestionType.MultiChoice, order, text, explanation, codeSnippet, options);

    private static Question NewChoiceQuestion(
        QuestionType type, int order, string text, string explanation, string? codeSnippet,
        (string Text, bool IsCorrect)[] options)
    {
        var question = new Question
        {
            Order = order,
            Type = type,
            Text = text,
            CodeSnippet = codeSnippet,
            Explanation = explanation
        };

        foreach (var (optionText, isCorrect) in options)
        {
            question.AnswerOptions.Add(new AnswerOption
            {
                Question = question,
                Text = optionText,
                IsCorrect = isCorrect
            });
        }

        return question;
    }

    private static Question ShortAnswer(
        int order, string text, string explanation, string expectedAnswer, bool isCaseSensitive = false)
    {
        var question = new Question
        {
            Order = order,
            Type = QuestionType.ShortAnswer,
            Text = text,
            Explanation = explanation
        };

        question.ShortAnswerKey = new ShortAnswerKey
        {
            Question = question,
            ExpectedAnswer = expectedAnswer,
            IsCaseSensitive = isCaseSensitive
        };

        return question;
    }
}
