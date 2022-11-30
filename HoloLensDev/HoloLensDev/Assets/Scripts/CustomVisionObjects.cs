using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//the object in here represent the serialized and de-serialized objects in unity 


//colons are used to denot whether a class is implementing a interface or not 
public class CustomVisionObjects : MonoBehaviour {


}

//WebRequest object for image data.  this is used like a java pojo when Json is serialized/deserialized
class MultipartObject : IMultipartFormSection
{
    public string sectionName { get; set; }

    public byte[] sectionData { get; set; }

    public string fileName { get; set; }

    public string contentType { get; set; }
}

//Json of all tags that are present inside of the project 
//tags are reference words that you can assign to objects in a project
public class Tags_RootObject
{
    public List<TagOfProject> Tags { get; set; }
    public int TotalTaggedImages { get; set; }
    public int TotalUntaggedImages { get; set; }
}
public class TagOfProject
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ImageCount { get; set; }
}

//Json of tag to associate to a image 
//contains list of tags since a image can have multple 
public class Tag_RootObject
{
    public List<Tag> Tags { get; set; }
}
public class Tag
{
    public string ImageId { get; set; }
    public string TagId { get; set; }
}

//Json of images submitted 
//contains object tha thost details about the images 
public class ImageRootObject
{
    public bool IsBatchSuccessful { get; set; }
    public List<SubmittedImage> Images{ get; set; } 
}
public class SubmittedImage
{
    public string SourceUrl { get; set; }
    public string Status { get; set; }
    public ImageObject Image { get; set; }
}
public class ImageObject
{
    public string ID { get; set; }
    public DateTime Created { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string ImageUri { get; set; }
    public string ThumbnailUri { get; set; }
}

//Json Serivce Iteration 
public class Iteration
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public string Status { get; set; }
    public string Created { get; set; }
    public string LastModified { get; set; }
    public string TrainedAt { get; set; }
    public string ProjectId { get; set; }
    public bool Exportable { get; set; }
    public string DomainId { get; set; }
}

//Predictions recieved from the service 
public class AnalysisRootObject
{
    public string id { get; set; }
    public string project { get; set; }
    public string iteration { get; set; }
    public DateTime created { get; set; }
    public List<Prediction> predictions { get; set; }
}
public class BoundingBox
{
    public double left { get; set; }
    public double top { get; set; }
    public double width { get; set; }
    public double height { get; set; }
}
public class Prediction
{
    public double probability { get; set; }
    public string tagId { get; set; }
    public string tagName { get; set; }
    public BoundingBox boundingBox { get; set; }
}
