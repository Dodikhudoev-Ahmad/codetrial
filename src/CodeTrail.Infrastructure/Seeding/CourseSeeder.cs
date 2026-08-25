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
        context.Courses.AddRange(
            BuildCSharpCourse(author),
            BuildSqlCourse(author),
            BuildPythonBasicsCourse(author),
            BuildJavascriptBasicsCourse(author),
            BuildTypescriptBasicsCourse(author),
            BuildGitBasicsCourse(author),
            BuildHtmlCssBasicsCourse(author),
            BuildJavaBasicsCourse(author),
            BuildOopBasicsCourse(author),
            BuildAlgorithmsBasicsCourse(author),
            BuildRestApiBasicsCourse(author),
            BuildDockerBasicsCourse(author));

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

    private static Course BuildPythonBasicsCourse(User author)
    {
        return NewCourse(
            title: "Python — основы синтаксиса",
            slug: "python-basics",
            description: "Первое знакомство с Python: переменные, типы данных, условия и циклы на примерах, которые можно сразу запустить.",
            level: CourseLevel.Beginner,
            language: "Python",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Переменные и типы данных",
"""
# Переменные и типы данных

В Python не нужно объявлять тип переменной заранее — интерпретатор определяет его автоматически по значению.

```python
name = "Алиса"      # str
age = 25             # int
height = 1.68         # float
is_student = True    # bool
```

Узнать тип значения можно функцией `type()`:

```python
print(type(age))  # <class 'int'>
```

Python — язык с динамической типизацией: одной и той же переменной можно присвоить значение другого типа.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какая функция возвращает тип значения переменной в Python?",
            "Функция type() возвращает объект класса, к которому принадлежит значение.",
            null,
            ("len()", false), ("type()", true), ("typeof()", false), ("kind()", false)),
        ShortAnswer(2,
            "Как называется тип данных для значений True и False?",
            "Логический тип в Python называется bool.",
            expectedAnswer: "bool")),

    NewLesson(2, "Условные операторы",
"""
# Условные операторы

Ветвление в Python строится на ключевых словах `if`, `elif`, `else`. Блоки выделяются отступами, а не скобками:

```python
score = 82

if score >= 90:
    grade = "A"
elif score >= 75:
    grade = "B"
else:
    grade = "C"

print(grade)  # B
```

Важно: отступ должен быть одинаковым внутри одного блока (обычно 4 пробела), иначе интерпретатор выдаст `IndentationError`.
""",
        xpReward: 10,
        SingleChoice(1,
            "Что определяет границы блока кода внутри if в Python?",
            "В Python блоки кода выделяются отступами, а не фигурными скобками.",
            null,
            ("Фигурные скобки {}", false), ("Отступы", true), ("Ключевое слово end", false), ("Точка с запятой", false)),
        SingleChoice(2,
            "Какое ключевое слово используется для дополнительного условия после if?",
            "elif — сокращение от 'else if', проверяется, если предыдущее условие ложно.",
            null,
            ("elseif", false), ("elif", true), ("else if", false), ("then", false))),

    NewLesson(3, "Списки и циклы",
"""
# Списки и циклы

Список (`list`) — упорядоченная изменяемая коллекция:

```python
fruits = ["яблоко", "банан", "вишня"]

for fruit in fruits:
    print(fruit)
```

Цикл `for` в Python перебирает элементы напрямую, без индексов. Если индекс нужен, используют `enumerate()`:

```python
for i, fruit in enumerate(fruits):
    print(i, fruit)
```

Добавить элемент можно методом `append()`, а длину списка узнать функцией `len()`.
""",
        xpReward: 15,
        SingleChoice(1,
            "Какой метод добавляет элемент в конец списка?",
            "list.append(x) добавляет элемент x в конец списка.",
            null,
            ("add()", false), ("push()", false), ("append()", true), ("insert()", false)),
        MultiChoice(2,
            "Какие из следующих утверждений про списки Python верны?",
            "Списки упорядочены и изменяемы; в одном списке могут храниться значения разных типов.",
            null,
            ("Список изменяем (mutable)", true), ("Список сохраняет порядок элементов", true), ("Список может содержать только один тип данных", false), ("Длину списка можно узнать через len()", true)))
            });
    }

    private static Course BuildJavascriptBasicsCourse(User author)
    {
        return NewCourse(
            title: "JavaScript — основы",
            slug: "javascript-basics",
            description: "Переменные, функции и работа с массивами в JavaScript — фундамент для дальнейшего изучения фронтенда.",
            level: CourseLevel.Beginner,
            language: "JavaScript",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Переменные: let, const, var",
"""
# Переменные: let, const, var

Современный JavaScript использует `let` и `const` вместо устаревшего `var`:

```javascript
let count = 0;
count = count + 1;

const PI = 3.14159;
// PI = 3; // ошибка: нельзя переприсвоить const
```

`let` и `const` имеют блочную область видимости (`{ }`), а `var` — функциональную, из-за чего `var` может приводить к неожиданным багам.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какое ключевое слово объявляет переменную, значение которой нельзя переприсвоить?",
            "const создаёт привязку, которую нельзя переприсвоить (хотя содержимое объекта/массива менять можно).",
            null,
            ("let", false), ("var", false), ("const", true), ("static", false)),
        SingleChoice(2,
            "Чем отличается область видимости let от var?",
            "let ограничен блоком {}, в котором объявлен, а var — всей функцией.",
            null,
            ("let блочная, var функциональная", true), ("var блочная, let функциональная", false), ("Отличий нет", false), ("let работает только в циклах", false))),

    NewLesson(2, "Функции и стрелочные функции",
"""
# Функции и стрелочные функции

Функцию можно объявить классическим способом или через стрелочный синтаксис:

```javascript
function add(a, b) {
  return a + b;
}

const multiply = (a, b) => a * b;
```

Стрелочные функции короче и не создают собственный `this`, что удобно внутри обработчиков событий и методов массивов.
""",
        xpReward: 10,
        ShortAnswer(1,
            "Какой оператор используется для создания стрелочной функции?",
            "Стрелка => отделяет параметры функции от её тела.",
            expectedAnswer: "=>"),
        SingleChoice(2,
            "Чем стрелочные функции отличаются от обычных в контексте this?",
            "Стрелочные функции не создают собственный this, а используют this из окружающего контекста.",
            null,
            ("Не создают собственный this", true), ("Всегда возвращают undefined", false), ("Не могут принимать аргументы", false), ("Работают только с числами", false))),

    NewLesson(3, "Методы массивов: map, filter",
"""
# Методы массивов: map, filter

`map` преобразует каждый элемент массива, `filter` оставляет только подходящие:

```javascript
const numbers = [1, 2, 3, 4, 5];

const doubled = numbers.map(n => n * 2);
// [2, 4, 6, 8, 10]

const even = numbers.filter(n => n % 2 === 0);
// [2, 4]
```

Оба метода не изменяют исходный массив, а возвращают новый — это важный принцип неизменяемости данных в функциональном стиле.
""",
        xpReward: 15,
        SingleChoice(1,
            "Что возвращает метод filter()?",
            "filter возвращает новый массив только с элементами, для которых колбэк вернул true.",
            null,
            ("Новый массив с отфильтрованными элементами", true), ("Число подходящих элементов", false), ("Изменённый исходный массив", false), ("true или false", false)),
        MultiChoice(2,
            "Какие из утверждений про map() верны?",
            "map создаёт новый массив той же длины, применяя функцию к каждому элементу, не мутируя исходный.",
            null,
            ("Возвращает новый массив", true), ("Длина результата равна длине исходного массива", true), ("Изменяет исходный массив", false), ("Может принимать функцию-колбэк", true)))
            });
    }

    private static Course BuildTypescriptBasicsCourse(User author)
    {
        return NewCourse(
            title: "TypeScript для начинающих",
            slug: "typescript-basics",
            description: "Статическая типизация поверх JavaScript: типы, интерфейсы и дженерики на практических примерах.",
            level: CourseLevel.Intermediate,
            language: "TypeScript",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Базовые типы",
"""
# Базовые типы

TypeScript добавляет статическую типизацию к JavaScript:

```typescript
let username: string = "Иван";
let age: number = 30;
let isActive: boolean = true;
let tags: string[] = ["admin", "user"];
```

Если тип можно определить из начального значения, TypeScript выведет его автоматически (type inference), и явно указывать тип не обязательно.
""",
        xpReward: 10,
        SingleChoice(1,
            "Как объявить массив строк в TypeScript?",
            "string[] — массив строк; также допустима запись Array<string>.",
            null,
            ("string[]", true), ("array(string)", false), ("[string]", false), ("str[]", false)),
        SingleChoice(2,
            "Что произойдёт, если не указать тип переменной, но задать начальное значение?",
            "TypeScript выводит тип автоматически на основе начального значения (type inference).",
            null,
            ("Ошибка компиляции", false), ("Тип будет выведен автоматически", true), ("Переменная станет any без предупреждений", false), ("Код не скомпилируется без явного типа", false))),

    NewLesson(2, "Интерфейсы",
"""
# Интерфейсы

Интерфейс описывает форму объекта:

```typescript
interface User {
  id: number;
  name: string;
  email?: string; // необязательное поле
}

function greet(user: User): string {
  return `Привет, ${user.name}!`;
}
```

Знак `?` после имени поля делает его необязательным. Если объект не соответствует интерфейсу, компилятор укажет на ошибку ещё до запуска кода.
""",
        xpReward: 10,
        ShortAnswer(1,
            "Какой символ делает поле интерфейса необязательным?",
            "Знак вопроса ? после имени поля обозначает, что оно необязательное.",
            expectedAnswer: "?"),
        SingleChoice(2,
            "Для чего используется interface в TypeScript?",
            "interface описывает ожидаемую структуру (форму) объекта.",
            null,
            ("Для описания формы объекта", true), ("Для создания циклов", false), ("Для импорта модулей", false), ("Для объявления констант", false))),

    NewLesson(3, "Дженерики",
"""
# Дженерики

Дженерики позволяют писать переиспользуемый код, сохраняя типобезопасность:

```typescript
function firstItem<T>(items: T[]): T {
  return items[0];
}

const num = firstItem([1, 2, 3]);      // T выведен как number
const str = firstItem(["a", "b"]);     // T выведен как string
```

Параметр типа `<T>` — это placeholder, который подставляется реальным типом при вызове функции.
""",
        xpReward: 15,
        SingleChoice(1,
            "Что обозначает <T> в объявлении функции?",
            "<T> — параметр типа (дженерик), placeholder для конкретного типа, который подставляется при вызове.",
            null,
            ("Параметр типа (дженерик)", true), ("Тип tuple", false), ("Ключевое слово ts", false), ("Комментарий", false)),
        MultiChoice(2,
            "Какие преимущества дают дженерики?",
            "Дженерики позволяют переиспользовать код для разных типов, сохраняя проверку типов на этапе компиляции.",
            null,
            ("Переиспользование кода для разных типов", true), ("Сохранение типобезопасности", true), ("Автоматическое исправление багов в рантайме", false), ("Ускорение выполнения кода в браузере", false)))
            });
    }

    private static Course BuildGitBasicsCourse(User author)
    {
        return NewCourse(
            title: "Git и контроль версий",
            slug: "git-basics",
            description: "Базовые команды Git: коммиты, ветки и слияния — необходимый минимум для командной разработки.",
            level: CourseLevel.Beginner,
            language: "Git",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Коммиты и статус репозитория",
"""
# Коммиты и статус репозитория

Git отслеживает изменения файлов через три состояния: рабочая директория, индекс (stage) и репозиторий.

```bash
git status          # что изменилось
git add file.txt     # добавить файл в индекс
git commit -m "Add feature"  # зафиксировать изменения
```

Коммит — это снимок состояния проекта с уникальным идентификатором (хешем) и сообщением, объясняющим, что изменилось и зачем.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какая команда показывает, какие файлы изменены, но ещё не добавлены в индекс?",
            "git status показывает состояние рабочей директории и индекса.",
            null,
            ("git log", false), ("git status", true), ("git diff --staged", false), ("git branch", false)),
        SingleChoice(2,
            "Что делает команда git add?",
            "git add помещает изменения в индекс (staging area) перед коммитом.",
            null,
            ("Добавляет файл в индекс перед коммитом", true), ("Создаёт новую ветку", false), ("Отправляет изменения на сервер", false), ("Удаляет файл из репозитория", false))),

    NewLesson(2, "Ветвление",
"""
# Ветвление

Ветки позволяют разрабатывать функциональность изолированно от основной линии:

```bash
git branch feature-login       # создать ветку
git checkout feature-login     # переключиться на неё
git checkout -b feature-login  # создать и сразу переключиться
```

Ветка `main` (или `master`) обычно содержит стабильный код, а разработка новых функций ведётся в отдельных ветках, которые потом сливаются обратно.
""",
        xpReward: 10,
        ShortAnswer(1,
            "Какой флаг у команды git checkout создаёт новую ветку и сразу на неё переключает?",
            "Флаг -b создаёт ветку и переключается на неё одной командой.",
            expectedAnswer: "-b"),
        SingleChoice(2,
            "Зачем нужны отдельные ветки в Git?",
            "Ветки изолируют разработку новой функциональности от стабильного кода в основной ветке.",
            null,
            ("Для изолированной разработки функциональности", true), ("Для увеличения скорости коммитов", false), ("Для автоматического тестирования", false), ("Для шифрования кода", false))),

    NewLesson(3, "Слияние и конфликты",
"""
# Слияние и конфликты

Слияние (`merge`) объединяет изменения из одной ветки в другую:

```bash
git checkout main
git merge feature-login
```

Если один и тот же участок файла менялся в обеих ветках по-разному, возникает конфликт. Git помечает его маркерами `<<<<<<<`, `=======`, `>>>>>>>` — конфликт нужно разрешить вручную и закоммитить результат.
""",
        xpReward: 15,
        SingleChoice(1,
            "Когда возникает конфликт слияния?",
            "Конфликт возникает, если один и тот же участок кода был изменён по-разному в обеих ветках.",
            null,
            ("Когда один участок кода изменён по-разному в двух ветках", true), ("При каждом git merge без исключений", false), ("Только при удалении файлов", false), ("Когда репозиторий пустой", false)),
        MultiChoice(2,
            "Какие маркеры Git использует для обозначения конфликта в файле?",
            "Git оборачивает конфликтующие участки маркерами <<<<<<<, ======= и >>>>>>>.",
            null,
            ("<<<<<<<", true), ("=======", true), (">>>>>>>", true), ("#######", false)))
            });
    }

    private static Course BuildHtmlCssBasicsCourse(User author)
    {
        return NewCourse(
            title: "HTML и CSS: вёрстка веб-страниц",
            slug: "html-css-basics",
            description: "Структура HTML-документа и оформление стилями CSS — с чего начинается любой сайт.",
            level: CourseLevel.Beginner,
            language: "HTML/CSS",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Структура HTML-документа",
"""
# Структура HTML-документа

Каждая HTML-страница состоит из тегов, вложенных друг в друга:

```html
<!DOCTYPE html>
<html>
  <head>
    <title>Моя страница</title>
  </head>
  <body>
    <h1>Заголовок</h1>
    <p>Абзац текста.</p>
  </body>
</html>
```

`<head>` содержит служебную информацию (заголовок вкладки, метаданные), а `<body>` — видимое содержимое страницы.
""",
        xpReward: 10,
        SingleChoice(1,
            "В каком теге размещается видимое содержимое страницы?",
            "<body> содержит весь контент, который отображается пользователю.",
            null,
            ("<head>", false), ("<body>", true), ("<title>", false), ("<meta>", false)),
        SingleChoice(2,
            "Какой тег задаёт заголовок вкладки браузера?",
            "<title>, расположенный внутри <head>, задаёт текст заголовка вкладки.",
            null,
            ("<h1>", false), ("<header>", false), ("<title>", true), ("<caption>", false))),

    NewLesson(2, "Селекторы CSS",
"""
# Селекторы CSS

CSS применяет стили к элементам через селекторы:

```css
p { color: navy; }          /* по тегу */
.highlight { background: yellow; }  /* по классу */
#header { font-size: 24px; }        /* по id */
```

Класс (`.`) можно применить к нескольким элементам, а id (`#`) должен быть уникален на странице. Специфичность id выше, чем у класса.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какой символ используется для селектора по классу в CSS?",
            "Точка (.) перед именем обозначает селектор класса.",
            null,
            ("#", false), (".", true), ("*", false), ("@", false)),
        MultiChoice(2,
            "Какие утверждения о селекторах верны?",
            "id уникален на странице и имеет более высокую специфичность, чем класс, который можно переиспользовать на многих элементах.",
            null,
            ("id должен быть уникален на странице", true), ("Класс можно применять к нескольким элементам", true), ("Специфичность id ниже, чем у класса", false), ("Селектор по тегу применяется ко всем элементам этого тега", true))),

    NewLesson(3, "Flexbox",
"""
# Flexbox

Flexbox — модель компоновки для выравнивания элементов в ряд или в столбец:

```css
.container {
  display: flex;
  justify-content: center;  /* выравнивание по главной оси */
  align-items: center;      /* выравнивание по поперечной оси */
  gap: 16px;
}
```

`justify-content` управляет распределением по главной оси (обычно горизонтальной), `align-items` — по поперечной (обычно вертикальной).
""",
        xpReward: 15,
        SingleChoice(1,
            "Какое свойство включает flex-контейнер?",
            "display: flex превращает элемент в flex-контейнер для его прямых потомков.",
            null,
            ("display: flex", true), ("position: flex", false), ("flex: on", false), ("layout: flex", false)),
        ShortAnswer(2,
            "Какое CSS-свойство задаёт отступ между flex-элементами?",
            "Свойство gap задаёт расстояние между элементами внутри flex- или grid-контейнера.",
            expectedAnswer: "gap"))
            });
    }

    private static Course BuildJavaBasicsCourse(User author)
    {
        return NewCourse(
            title: "Java — основы",
            slug: "java-basics",
            description: "Классы, объекты и базовый синтаксис Java — язык, на котором построена значительная часть корпоративного ПО.",
            level: CourseLevel.Beginner,
            language: "Java",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Классы и объекты",
"""
# Классы и объекты

Java — объектно-ориентированный язык: весь код живёт внутри классов.

```java
public class Dog {
    String name;

    public void bark() {
        System.out.println(name + " говорит: Гав!");
    }
}

Dog myDog = new Dog();
myDog.name = "Рекс";
myDog.bark();
```

Класс — это шаблон, а объект (`new Dog()`) — конкретный экземпляр, созданный по этому шаблону.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какое ключевое слово создаёт новый объект в Java?",
            "Оператор new выделяет память и создаёт новый экземпляр класса.",
            null,
            ("new", true), ("create", false), ("make", false), ("object", false)),
        SingleChoice(2,
            "Чем класс отличается от объекта?",
            "Класс — шаблон/описание, объект — конкретный экземпляр, созданный по этому шаблону.",
            null,
            ("Класс — шаблон, объект — его экземпляр", true), ("Это одно и то же", false), ("Объект существует только на этапе компиляции", false), ("Класс создаётся из объекта", false))),

    NewLesson(2, "Типы данных и переменные",
"""
# Типы данных и переменные

В Java тип переменной указывается явно и не может измениться (статическая типизация):

```java
int age = 25;
double price = 19.99;
boolean isActive = true;
String name = "Игорь";
```

`int`, `double`, `boolean` — примитивные типы (хранят значение напрямую), а `String` — ссылочный тип (объект).
""",
        xpReward: 10,
        SingleChoice(1,
            "Какой из типов является примитивным в Java?",
            "int — примитивный тип, хранящий целое число напрямую, в отличие от String, который является объектом.",
            null,
            ("String", false), ("int", true), ("ArrayList", false), ("Object", false)),
        MultiChoice(2,
            "Какие типы данных являются примитивными в Java?",
            "int, double и boolean — примитивные типы; String — ссылочный тип (объект).",
            null,
            ("int", true), ("double", true), ("boolean", true), ("String", false))),

    NewLesson(3, "Условия и циклы",
"""
# Условия и циклы

Синтаксис управляющих конструкций Java похож на C и JavaScript:

```java
for (int i = 0; i < 5; i++) {
    if (i % 2 == 0) {
        System.out.println(i + " чётное");
    }
}
```

Цикл `for` состоит из трёх частей: инициализация, условие продолжения и шаг — все разделены точкой с запятой.
""",
        xpReward: 15,
        SingleChoice(1,
            "Сколько частей у классического цикла for в Java, разделённых точкой с запятой?",
            "Инициализация, условие и шаг — три части, разделённые ';'.",
            null,
            ("Две", false), ("Три", true), ("Четыре", false), ("Одна", false)),
        ShortAnswer(2,
            "Какой оператор проверяет остаток от деления в Java?",
            "Оператор % возвращает остаток от деления, часто используется для проверки чётности.",
            expectedAnswer: "%"))
            });
    }

    private static Course BuildOopBasicsCourse(User author)
    {
        return NewCourse(
            title: "Объектно-ориентированное программирование",
            slug: "oop-basics",
            description: "Инкапсуляция, наследование и полиморфизм на примерах C# — ключевые принципы ООП.",
            level: CourseLevel.Intermediate,
            language: "C#",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Инкапсуляция",
"""
# Инкапсуляция

Инкапсуляция скрывает внутреннее состояние объекта, предоставляя контролируемый доступ через свойства и методы:

```csharp
public class BankAccount
{
    private decimal balance;

    public decimal Balance => balance;

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Сумма должна быть положительной");
        balance += amount;
    }
}
```

Поле `balance` помечено `private` и недоступно напрямую снаружи класса — изменить его можно только через метод `Deposit`, который проверяет корректность значения.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какой модификатор доступа скрывает поле от кода вне класса?",
            "private ограничивает доступ только внутри объявляющего класса.",
            null,
            ("public", false), ("private", true), ("protected", false), ("internal", false)),
        SingleChoice(2,
            "Зачем нужна инкапсуляция?",
            "Инкапсуляция защищает внутреннее состояние объекта от некорректных изменений извне.",
            null,
            ("Чтобы скрыть и защитить внутреннее состояние объекта", true), ("Чтобы ускорить выполнение кода", false), ("Чтобы уменьшить размер файла", false), ("Чтобы отключить наследование", false))),

    NewLesson(2, "Наследование",
"""
# Наследование

Наследование позволяет одному классу перенимать поведение другого:

```csharp
public class Animal
{
    public string Name { get; set; }
    public virtual void Speak() => Console.WriteLine("...");
}

public class Cat : Animal
{
    public override void Speak() => Console.WriteLine("Мяу!");
}
```

Класс `Cat` наследует все члены `Animal` и переопределяет метод `Speak` (для этого исходный метод должен быть помечен `virtual`, а переопределяющий — `override`).
""",
        xpReward: 10,
        SingleChoice(1,
            "Какое ключевое слово используется для переопределения метода базового класса в C#?",
            "override переопределяет метод, помеченный как virtual в базовом классе.",
            null,
            ("new", false), ("override", true), ("base", false), ("implements", false)),
        ShortAnswer(2,
            "Каким модификатором должен быть помечен метод базового класса, чтобы его можно было переопределить?",
            "Метод должен быть помечен virtual, чтобы производный класс мог его переопределить через override.",
            expectedAnswer: "virtual")),

    NewLesson(3, "Полиморфизм",
"""
# Полиморфизм

Полиморфизм позволяет работать с объектами разных производных классов через общий базовый тип:

```csharp
List<Animal> animals = new() { new Cat(), new Dog() };

foreach (var animal in animals)
{
    animal.Speak(); // вызовется переопределённая версия для каждого типа
}
```

Хотя переменная имеет тип `Animal`, во время выполнения вызывается тот метод `Speak`, который переопределён в реальном (runtime) типе объекта.
""",
        xpReward: 15,
        SingleChoice(1,
            "Что демонстрирует пример с List<Animal>, содержащим Cat и Dog?",
            "Вызов через базовый тип приводит к выполнению переопределённого метода конкретного (runtime) типа объекта — это и есть полиморфизм.",
            null,
            ("Полиморфизм", true), ("Инкапсуляцию", false), ("Абстракцию данных", false), ("Статическую типизацию", false)),
        MultiChoice(2,
            "Какие условия нужны, чтобы переопределение метода работало полиморфно в C#?",
            "Метод базового класса должен быть virtual (или abstract), а метод производного класса — override.",
            null,
            ("Метод базового класса помечен virtual", true), ("Метод производного класса помечен override", true), ("Оба метода должны быть static", false), ("Классы должны быть в одном файле", false)))
            });
    }

    private static Course BuildAlgorithmsBasicsCourse(User author)
    {
        return NewCourse(
            title: "Алгоритмы и структуры данных",
            slug: "algorithms-basics",
            description: "Сложность алгоритмов, сортировки и основные структуры данных — база для собеседований и эффективного кода.",
            level: CourseLevel.Advanced,
            language: "C#",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Нотация O-большое",
"""
# Нотация O-большое

О-нотация описывает, как растёт время выполнения алгоритма относительно размера входных данных `n`:

- `O(1)` — константное время (доступ по индексу массива)
- `O(n)` — линейное время (проход по всем элементам)
- `O(n²)` — квадратичное время (вложенные циклы, например пузырьковая сортировка)
- `O(log n)` — логарифмическое время (бинарный поиск)

Нотация описывает поведение в худшем случае при росте `n`, а не точное количество операций.
""",
        xpReward: 15,
        SingleChoice(1,
            "Какую сложность имеет доступ к элементу массива по индексу?",
            "Доступ по индексу выполняется за константное время O(1) независимо от размера массива.",
            null,
            ("O(1)", true), ("O(n)", false), ("O(n²)", false), ("O(log n)", false)),
        SingleChoice(2,
            "Какую сложность обычно имеет бинарный поиск в отсортированном массиве?",
            "Бинарный поиск делит область поиска пополам на каждом шаге — O(log n).",
            null,
            ("O(n)", false), ("O(n²)", false), ("O(log n)", true), ("O(1)", false))),

    NewLesson(2, "Сортировки",
"""
# Сортировки

Разные алгоритмы сортировки имеют разную сложность и подходят для разных случаев:

```csharp
// Пузырьковая сортировка — O(n²), проста, но неэффективна
for (int i = 0; i < arr.Length - 1; i++)
    for (int j = 0; j < arr.Length - i - 1; j++)
        if (arr[j] > arr[j + 1])
            (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
```

Более эффективные алгоритмы, такие как быстрая сортировка (QuickSort) и сортировка слиянием (MergeSort), работают за `O(n log n)` в среднем случае.
""",
        xpReward: 15,
        SingleChoice(1,
            "Какова сложность пузырьковой сортировки в худшем случае?",
            "Пузырьковая сортировка использует вложенные циклы, что даёт квадратичную сложность O(n²).",
            null,
            ("O(n)", false), ("O(n log n)", false), ("O(n²)", true), ("O(1)", false)),
        MultiChoice(2,
            "Какие алгоритмы сортировки работают в среднем за O(n log n)?",
            "QuickSort и MergeSort в среднем случае имеют сложность O(n log n), в отличие от пузырьковой сортировки.",
            null,
            ("QuickSort", true), ("MergeSort", true), ("Пузырьковая сортировка", false), ("Сортировка выбором", false))),

    NewLesson(3, "Стек и очередь",
"""
# Стек и очередь

Стек (`Stack`) работает по принципу LIFO (последним пришёл — первым вышел):

```csharp
var stack = new Stack<int>();
stack.Push(1);
stack.Push(2);
stack.Pop(); // вернёт 2
```

Очередь (`Queue`) работает по принципу FIFO (первым пришёл — первым вышел):

```csharp
var queue = new Queue<int>();
queue.Enqueue(1);
queue.Enqueue(2);
queue.Dequeue(); // вернёт 1
```
""",
        xpReward: 15,
        SingleChoice(1,
            "По какому принципу работает стек?",
            "Stack — LIFO: Last In, First Out — последний добавленный элемент извлекается первым.",
            null,
            ("FIFO", false), ("LIFO", true), ("Случайный порядок", false), ("По приоритету", false)),
        ShortAnswer(2,
            "Как называется структура данных, работающая по принципу FIFO?",
            "Очередь (Queue) работает по принципу FIFO: первый пришёл — первый вышел.",
            expectedAnswer: "очередь"))
            });
    }

    private static Course BuildRestApiBasicsCourse(User author)
    {
        return NewCourse(
            title: "REST API и веб-сервисы",
            slug: "rest-api-basics",
            description: "Принципы REST, HTTP-методы и коды состояний — как устроено взаимодействие клиента и сервера в вебе.",
            level: CourseLevel.Intermediate,
            language: "HTTP",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "HTTP-методы",
"""
# HTTP-методы

REST API строится вокруг стандартных HTTP-методов, каждый со своим смыслом:

- `GET` — получить данные (не изменяет состояние сервера)
- `POST` — создать новый ресурс
- `PUT` — полностью заменить существующий ресурс
- `PATCH` — частично обновить ресурс
- `DELETE` — удалить ресурс

`GET` называют "безопасным" (safe) методом: повторный вызов не должен ничего менять на сервере.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какой HTTP-метод используется для создания нового ресурса?",
            "POST используется для создания новых ресурсов на сервере.",
            null,
            ("GET", false), ("POST", true), ("DELETE", false), ("HEAD", false)),
        SingleChoice(2,
            "Какой метод считается 'безопасным' — то есть не должен изменять состояние сервера?",
            "GET предназначен только для чтения данных и не должен иметь побочных эффектов.",
            null,
            ("GET", true), ("POST", false), ("PUT", false), ("DELETE", false))),

    NewLesson(2, "Коды состояния HTTP",
"""
# Коды состояния HTTP

Каждый ответ сервера содержит код состояния, сгруппированный по диапазонам:

- `2xx` — успех (`200 OK`, `201 Created`, `204 No Content`)
- `4xx` — ошибка клиента (`400 Bad Request`, `401 Unauthorized`, `404 Not Found`)
- `5xx` — ошибка сервера (`500 Internal Server Error`)

Код `401` означает, что пользователь не аутентифицирован, а `403` — что он аутентифицирован, но не имеет прав на это действие.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какой код состояния означает, что ресурс не найден?",
            "404 Not Found означает, что запрошенный ресурс не существует.",
            null,
            ("200", false), ("401", false), ("404", true), ("500", false)),
        MultiChoice(2,
            "Какие из перечисленных кодов относятся к ошибкам клиента (4xx)?",
            "400, 401 и 404 — коды 4xx (ошибки клиента); 500 — ошибка сервера (5xx).",
            null,
            ("400", true), ("401", true), ("404", true), ("500", false))),

    NewLesson(3, "Проектирование эндпоинтов",
"""
# Проектирование эндпоинтов

Хороший REST API использует существительные во множественном числе для коллекций ресурсов и вложенность для связанных сущностей:

```
GET    /api/courses           — список курсов
GET    /api/courses/{id}      — один курс
POST   /api/courses           — создать курс
GET    /api/courses/{id}/lessons  — уроки конкретного курса
```

Действия (глаголы) обычно не кладут в URL — вместо `/api/deleteCourse` используют `DELETE /api/courses/{id}`, где само действие выражено HTTP-методом.
""",
        xpReward: 15,
        SingleChoice(1,
            "Как правильно спроектировать URL для удаления курса в REST-стиле?",
            "В REST действие выражается HTTP-методом (DELETE), а не глаголом в URL.",
            null,
            ("DELETE /api/courses/{id}", true), ("GET /api/deleteCourse/{id}", false), ("POST /api/courses/delete", false), ("PUT /api/remove/{id}", false)),
        SingleChoice(2,
            "Как правильно назвать эндпоинт для получения списка курсов?",
            "REST-конвенция использует существительные во множественном числе для коллекций ресурсов.",
            null,
            ("/api/course", false), ("/api/courses", true), ("/api/getCourses", false), ("/api/course-list", false)))
            });
    }

    private static Course BuildDockerBasicsCourse(User author)
    {
        return NewCourse(
            title: "Docker и контейнеризация",
            slug: "docker-basics",
            description: "Образы, контейнеры и Dockerfile — как упаковать приложение для запуска в любой среде.",
            level: CourseLevel.Intermediate,
            language: "Docker",
            author: author,
            lessons: new[]
            {
    NewLesson(1, "Образы и контейнеры",
"""
# Образы и контейнеры

Docker-образ (image) — неизменяемый шаблон, включающий код приложения и все его зависимости. Контейнер — запущенный экземпляр образа:

```bash
docker build -t myapp .     # собрать образ из Dockerfile
docker run -d -p 8080:80 myapp  # запустить контейнер
```

Один и тот же образ можно запускать в неограниченном числе контейнеров, каждый из которых изолирован от остальных.
""",
        xpReward: 10,
        SingleChoice(1,
            "Чем контейнер отличается от образа?",
            "Образ — неизменяемый шаблон, контейнер — его запущенный экземпляр.",
            null,
            ("Контейнер — запущенный экземпляр образа", true), ("Это синонимы", false), ("Образ создаётся из контейнера", false), ("Контейнер существует только на этапе сборки", false)),
        ShortAnswer(2,
            "Какой командой запускается контейнер из уже собранного образа?",
            "docker run создаёт и запускает контейнер на основе указанного образа.",
            expectedAnswer: "docker run")),

    NewLesson(2, "Dockerfile",
"""
# Dockerfile

Dockerfile — текстовый файл с инструкциями для сборки образа:

```dockerfile
FROM node:20
WORKDIR /app
COPY package.json .
RUN npm install
COPY . .
CMD ["npm", "start"]
```

`FROM` задаёт базовый образ, `RUN` выполняет команду на этапе сборки, а `CMD` определяет команду, которая выполнится при запуске контейнера.
""",
        xpReward: 10,
        SingleChoice(1,
            "Какая инструкция Dockerfile задаёт базовый образ?",
            "FROM указывает базовый образ, на основе которого строится новый.",
            null,
            ("FROM", true), ("BASE", false), ("IMAGE", false), ("START", false)),
        SingleChoice(2,
            "В чём разница между RUN и CMD в Dockerfile?",
            "RUN выполняется при сборке образа, а CMD задаёт команду, которая выполнится при запуске контейнера.",
            null,
            ("RUN — при сборке, CMD — при запуске контейнера", true), ("Это полные синонимы", false), ("CMD — при сборке, RUN — при запуске", false), ("RUN используется только для копирования файлов", false))),

    NewLesson(3, "Тома и сети",
"""
# Тома и сети

Контейнеры по умолчанию эфемерны — данные внутри них пропадают при удалении. Тома (volumes) решают эту проблему:

```bash
docker run -v mydata:/var/lib/data myapp
```

Для связи между несколькими контейнерами (например, приложение + база данных) используют Docker-сети или `docker-compose`, который описывает несколько сервисов в одном YAML-файле.
""",
        xpReward: 15,
        SingleChoice(1,
            "Для чего нужны тома (volumes) в Docker?",
            "Тома обеспечивают сохранение данных за пределами жизненного цикла контейнера.",
            null,
            ("Для сохранения данных за пределами контейнера", true), ("Для ускорения сборки образа", false), ("Для шифрования сети", false), ("Для запуска нескольких образов одновременно", false)),
        ShortAnswer(2,
            "Каким инструментом обычно описывают несколько связанных контейнеров в одном YAML-файле?",
            "docker-compose позволяет описать несколько сервисов и их связи в одном файле docker-compose.yml.",
            expectedAnswer: "docker-compose"))
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
