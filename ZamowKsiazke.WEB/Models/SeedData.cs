using Microsoft.EntityFrameworkCore;
using ZamowKsiazke.Data;

namespace ZamowKsiazke.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ZamowKsiazkeContext(serviceProvider.GetRequiredService<DbContextOptions<ZamowKsiazkeContext>>()))
            {
                if (context.Book.Any())
                {
                    return;    
                }

                context.Book.AddRange(
                    new Book
                    {
                        Title = "Wiedźmin Rozdroże Kruków",
                        Language = "Polski",
                        ISBN = "9789129688313",
                        DatePublished = DateTime.Parse("2024-12-10"),
                        Price = 49,
                        Author = "Andrzej Sapkowski",
                        Description = "Oto słowa Proroka: zaprawdę powiadam wam, że nastanie chaos w wielu miejscach i ogień często będzie wybuchał. I oto sprawdziło się proroctwo i oczom naszym stało się ono widome: niewiasty porodziły potwory. Wiedźmini są tymi potworami, heroldami chaosu i zguby.",
                        ImageUrl = "/images/rozdrozekrukow.jpg"
                    },

                    new Book
                    {
                        Title = "Wiedźmin, Dzikie zwierzęta. Tom 8",
                        Language = "Polski",
                        ISBN = "9780261102354",
                        DatePublished = DateTime.Parse("2024-12-08"),
                        Price = 39,
                        Author = "Andrzej Sapkowski",
                        Description = "Geralt wdaje się w sprzeczkę ze swoim zleceniodawcą, a gdy argumenty zostają zastąpione przez miecze i strzały, wiedźmin musi szukać ucieczki w morzu.",
                        ImageUrl = "/images/dzikiezwierzeta.jpg"
                    },

                    new Book
                    {
                        Title = "Wiedźmin Ziarno prawdy",
                        Language = "Polski",
                        ISBN = "9780062068408",
                        DatePublished = DateTime.Parse("2024-12-06"),
                        Price = 29,
                        Author = "Andrzej Sapkowski",
                        Description = "Geralt w poszukiwaniu zlecenia trafia do tajemniczego starego dworu położonego w odludnej okolicy.",
                        ImageUrl = "/images/ziarnoprawdy.jpg"
                    },

                    new Book
                    {
                        Title = "Im mroczniej tym lepiej",
                        Language = "Polski",
                        ISBN = "9780021368408",
                        DatePublished = DateTime.Parse("2024-10-02"),
                        Price = 59,
                        Author = "Stephen King",
                        Description = "12 prawdziwych rarytasów od króla horroru. Od krótkich nowel, przez dłuższe opowiadania, po mini powieści. Od klasycznych horrorów, przez kryminały, po prozę obyczajową. Od Derry, przez Castle Rock, po Florydę. Od dreszczy zgrozy, przez dramatyczne napięcie, po łzy rozbawienia.",
                        ImageUrl = "/images/immroczniejtymlepiej.jpg"
                    },

                    new Book
                    {
                        Title = "Instytut",
                        Language = "Polski",
                        ISBN = "9780765068408",
                        DatePublished = DateTime.Parse("2024-10-04"),
                        Price = 59,
                        Author = "Stephen King",
                        Description = "Instytut Mocny jak To, przerażający jak Podpalaczka! Stephen King powraca do motywów znanych z jego najlepszych książek.",
                        ImageUrl = "/images/instytut.jpg"
                    },

                    new Book
                    {
                        Title = "Carrie",
                        Language = "Polski",
                        ISBN = "9780307386458",
                        DatePublished = DateTime.Parse("2024-10-10"),
                        Price = 29,
                        Author = "Stephen King",
                        Description = "Specjalne wydanie bestsellerowej powieści Carrie, która w 2024 r. obchodzi pięćdziesiątą rocznicę publikacji. W oprawie zintegrowanej, z barwionymi brzegami i przedmową autorstwa Margaret Atwood.",
                        ImageUrl = "/images/carrie.jpg"
                    },

                    new Book
                    {
                        Title = "Szepty spoza nicości",
                        Language = "Polski",
                        ISBN = "9780062068987",
                        DatePublished = DateTime.Parse("2022-10-04"),
                        Price = 29,
                        Author = "Remigiusz Mróz",
                        Description = "Seweryn Zaorski po raz ostatni wraca do Żeromic, by sprzedać dom i na dobre zostawić za sobą tragiczną przeszłość.",
                        ImageUrl = "/images/szepty.jpg"
                    },

                    new Book
                    {
                        Title = "Morderstwo w Mezopotamii",
                        Language = "Polski",
                        ISBN = "9780062068546",
                        DatePublished = DateTime.Parse("2019-06-04"),
                        Price = 27,
                        Author = "Agatha Christie",
                        Description = "To kryminalna historia o tym jak jeden wakacyjny wyjazd może zamienić się w nieskończoną serię problemów.",
                        ImageUrl = "/images/mezopotamia.jpg"
                    },

                    new Book
                    {
                        Title = "Venom II",
                        Language = "Polski",
                        ISBN = "9780062068123",
                        DatePublished = DateTime.Parse("2021-02-02"),
                        Price = 29,
                        Author = "Aleksandra Kołaciuk",
                        Description = "Drugi tom bestsellerowej serii! Historia lubi się powtarzać, Vivian przekonuje się o tym na własnej skórze.",
                        ImageUrl = "/images/venom2.jpg"
                    },

                    new Book
                    {
                        Title = "Pacjentka",
                        Language = "Polski",
                        ISBN = "9780062068321",
                        DatePublished = DateTime.Parse("2021-03-04"),
                        Price = 39,
                        Author = "Michaelides Alex",
                        Description = "Czy człowiek może zmienić się w jednej chwili o 180 stopni? Dowiedz się tego z jednego z najbardziej wyczekiwanych w 2019 roku thrillerów psychologicznych – „Pacjentka” autorstwa Alexa Michaelidesa.",
                        ImageUrl = "/images/pacjentka.jpg"
                    },

                    new Book
                    {
                        Title = "Głusza",
                        Language = "Polski",
                        ISBN = "9780062068357",
                        DatePublished = DateTime.Parse("2024-10-04"),
                        Price = 49,
                        Author = "Mark Edwards",
                        Description = "Wypoczynek z dala od cywilizacji zmieni się w wakacyjny koszmar.",
                        ImageUrl = "/images/glusza.jpg"
                    },

                    new Book
                    {
                        Title = "Cienie pośród mroku Tom 6",
                        Language = "Polski",
                        ISBN = "9780062062268",
                        DatePublished = DateTime.Parse("2024-12-05"),
                        Price = 59,
                        Author = "Remigiusz Mróz",
                        Description = "W życiu Seweryna i Burzy nie mogło wydarzyć się nic złego, nie kiedy wszystko wreszcie zaczęło im się układać. Nic nie zapowiadało nawałnicy, która miała na nich spaść.",
                        ImageUrl = "/images/cienieposrodmroku.jpg"
                    },

                    new Book
                    {
                        Title = "Debiut",
                        Language = "Polski",
                        ISBN = "9780062068000",
                        DatePublished = DateTime.Parse("2023-11-04"),
                        Price = 25,
                        Author = "Paula Świst",
                        Description = "W relacjach Aresa i Niny zachodzi znacząca zmiana. Do tej pory można je było określić jednym słowem: skomplikowane. Teraz stały się bardzo skomplikowane. A nawet bardziej niż bardzo.",
                        ImageUrl = "/images/debiut.jpg"
                    },

                    new Book
                    {
                        Title = "Cienie",
                        Language = "Polski",
                        ISBN = "9780062068444",
                        DatePublished = DateTime.Parse("2023-09-09"),
                        Price = 55,
                        Author = "Wojciech Chmielarz",
                        Description = "Nowa powieść Wojciecha Chmielarza, laureata nagrody Wielkiego Kalibru. Komisarz Jakub Mortka powraca, by rozwikłać tajemnicze zabójstwo.",
                        ImageUrl = "/images/cienie.jpg"
                    },

                    new Book
                    {
                        Title = "Opowiastki na zimowe wieczory. Kubuś i Przyjaciele",
                        Language = "Polski",
                        ISBN = "9780062988334",
                        DatePublished = DateTime.Parse("2024-09-09"),
                        Price = 35,
                        Author = "Katarzyna Łączyńska",
                        Description = "PRZYJAŹŃ, RADOŚĆ I MOC PRZYGÓD W STUMILOWYM LESIE.",
                        ImageUrl = "/images/kubuspuchatek.jpg"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
