using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Achievement {

    [Key]
    public int id { get; set; }
    [ForeignKey("User")]
    public int? userId { get; set; }
    public int? reviewsLeft { get; set; } = 0;
    public int? visits { get; set; } = 0;
    public int? citiesVisited { get; set; } = 0;
    public int? bookmarks { get; set; } = 0;
    public static void UpdateUserAchievements(FocusFinderDbContext dbContext, int userId, string actionType)
    {
        var achievement = dbContext.Achievements.FirstOrDefault(a => a.userId == userId);

        if (achievement == null)
        {
            achievement = new Achievement { userId = userId, reviewsLeft = 0, visits = 0, citiesVisited = 0, bookmarks = 0 };
            dbContext.Achievements.Add(achievement);
        }

        switch (actionType)
        {
            case "visit":
                achievement.visits += 1;
                break;
            case "review":
                achievement.reviewsLeft += 1;
                break;
            case "bookmark":
                achievement.bookmarks += 1;
                break;
            case "newCity":
                achievement.citiesVisited += 1;
                break;
        }

        dbContext.SaveChanges();
    }
}
}
