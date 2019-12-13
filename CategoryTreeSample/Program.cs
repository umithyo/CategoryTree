using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      var r = new Random();
      List<Category> categories = new List<Category>();
      categories.Seed(options =>
      {
        options.ChildrenCount = 2;
        options.DepthDefinition.Depth = 8;
        options.ParentCount = 3 ;

      });
      categories.AppendChildren();
      var s = categories.Where(x => x.Parent == null).ToList().GetTree();
      Console.Write(s);
    }
  }

  public class DepthDefinition
  {
    public int Depth { get; set; }
    public int MaxDepth { get; set; }
    public int MaxChildren { get; set; }
  }

  public class SeedOptions
  {
    public int ParentCount { get; set; }
    public int ChildrenCount { get; set; }
    public DepthDefinition DepthDefinition { get; set; } = new DepthDefinition();
  }

  public class Category
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public Category Parent { get; set; }
    public List<Category> Children { get; set; } = new List<Category>();
    public DepthDefinition DepthDefinition { get; set; } = new DepthDefinition();

    public Category()
    {

    }

    public Category(bool withDepthDefinitons)
    {
      if (withDepthDefinitons)
        this.DepthDefinition = new DepthDefinition();
    }
  }

  static class CategoryTree
  {
    public static string Repeat(this string s, int n)
    {
      return new StringBuilder(s.Length * n)
                      .AppendJoin(s, new string[n + 1])
                      .ToString();
    }



    public static List<Category> Seed(this List<Category> list, Action<SeedOptions> options)
    {
      var r = new Random();
      var _options = new SeedOptions();
      options(_options);
      for (int i = 1; i <= _options.ParentCount; i++)
      {
        var parentCat = new Category { Name = "Parent", Parent = null, Id = i.ToString() };
        parentCat.DepthDefinition.MaxChildren = r.Next(3, 7);
        list.Add(parentCat);
        parentCat.SeedChildren(list, _options);
      }
      return list;
    }

    private static void SeedChildren(this Category parentCat, List<Category> root, SeedOptions options, int? depth = null)
    {
      var r = new Random();
      depth = (depth != null ? depth : ((parentCat.DepthDefinition != null && parentCat.DepthDefinition.Depth > 0) ? parentCat.DepthDefinition.Depth : options.DepthDefinition.Depth)) - 1;
      for (int k = 1; k <= ((parentCat.DepthDefinition != null && parentCat.DepthDefinition.MaxChildren > 0) ? parentCat.DepthDefinition.MaxChildren : options.ChildrenCount); k++)
      {
        var child = new Category(true) { Name = $"Child", Parent = parentCat, Id = $"{parentCat.Id}_{k}" };
        child.DepthDefinition.MaxDepth = r.Next(4, 8);
        child.DepthDefinition.MaxChildren = r.Next(2, 4);
        if (depth > 0)
        {
          root.Add(child);
          child.SeedChildren(root, options, depth);
        }
      }
    }


    public static void AppendChildren(this List<Category> root)
    {
      foreach (var category in root)
      {
        if (category.Parent != null)
          category.Parent.Children.Add(category);
      }
    }

    public static StringBuilder GetTree(this List<Category> root)
    {
      StringBuilder sb = new StringBuilder();
      foreach (var parent in root)
      {
        sb.Append($"{parent.Name}_{parent.Id}\n");
        if (parent.Children.Count > 0)
        {
          parent.GetChildTree(sb);
        }
      }
      return sb;
    }

    private static StringBuilder GetChildTree(this Category parent, StringBuilder stringBuilder, int? depth = null)
    {
      StringBuilder sb = stringBuilder ?? new StringBuilder();
      foreach (var child in parent.Children)
      {
        int? maxDepth = null;
        if (child.DepthDefinition != null && child.DepthDefinition.MaxDepth > 0)
          maxDepth = child.DepthDefinition.MaxDepth;

        depth = depth ?? 1;
        if (maxDepth != null ? depth < maxDepth : true)
        {
          sb.Append($"|{"-".Repeat(depth.Value)}>{child.Name} Level: {child.Id}\n");
          if (child.Children.Count > 0)
          {
            child.GetChildTree(sb, depth + 1);
          }
        }
      }
      return sb;
    }
  }
}