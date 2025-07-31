using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    public List<string> tags; // List of all tags this object could have.


    // Methods you can use for the tag system! :D
    public bool HasTag(string tag) // Like CompareTag("Bla")
    {
        return tags.Contains(tag);
    }
    
    public void AddTag(string tag) // Add a tag to the object.
    {
        if (!HasTag(tag))
        {
            tags.Add(tag);
        }
        else
        {
            Debug.Log("ERROR: " + gameObject.name + "already has the Tag " + tag + "you wanted to add");
        }
    }

    public void RemoveTag(string tag) // Remove Tag from object.
    {
        if (HasTag(tag))
        {
            tags.Remove(tag);
        }
        else
        {
            Debug.Log("ERROR: " + gameObject.name + "doesn't have the Tag " + tag + "you wanted to remove");
        }
    }

    public void ReturnTags() // For Debug.
    {
        foreach (var x in tags)
        {
            Debug.Log(x.ToString());
        }
    }   
}
