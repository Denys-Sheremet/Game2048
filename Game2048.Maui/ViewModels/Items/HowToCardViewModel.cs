using System;
using System.Collections.Generic;
using System.Text;

namespace Game2048.Maui.ViewModels.Items;

public class HowToCardViewModel
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string ImageSource { get; set; }
}
