﻿
public enum NetworkStatus { Disconnected, Client, Server, ListeningUDP };
public enum Level { Level_Venedig, Level_Rom}

public enum eFood
{
    None,
    Pizza,
    Pasta,
    Soup,
    Salad,
    Fish
}

public enum eTableState
{
    Free,
    ReadingMenu,
    WaitingForOrder,
    WaitingForFood,
    Eating,
    WaitingForClean
}

public enum eStatisfaction
{
    Good,
    Neutral,
    Displeased,
    Angry
}

public enum eCarryableType
{
    //Empty,
    Food,
    Customer,
    Dishes
}

public enum eCustomers
{
    Normal,
    Snob
}
