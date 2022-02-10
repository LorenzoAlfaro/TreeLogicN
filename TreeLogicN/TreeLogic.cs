using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeLogicN
{
    public static class AuxFunctions
    {
        public static bool Checked(string a, List<string> b)
        {
            foreach (string currentString in b)
            {
                if (currentString == a)
                {
                    return true;
                }
            }
            return false;
        }
        public static List<string> copyList(List<string> a)
        {
            List<string> cloneList = new List<string>();

            foreach (string originalString in a)
            {
                cloneList.Add(originalString);
            }
            return cloneList;
        }
        public static string FirstName(string a, char b, string c = "")
        {
            if (c == "")
            {
                return a.Split(b)[0];
            }
            else
            {
                return a.Split(new string[] { c }, StringSplitOptions.None)[0];
            }
        }
        
        
    }
    
    /// <summary>
    /// Base class for the tree structure
    /// </summary>
    public class Node 
    {
        public string Trunk;
        public List<Node> Branches;
        public Node()
        {
            Branches = new List<Node>();
        }
    }

    public class Path
    {
        public List<string> Silaba;
    }

    public class DeviceInterface
    {
        DeviceInterface(string path)
        {
            string[] lines;
            lines = File.ReadAllLines(path);
            paths = lines.ToList();
        }

        public List<string> paths;
    }

    // TODO use namespace instead of a static class
    public static class TreeLogic
    {
        #region Paths logic

        /// <summary>
        /// Function to check if a path to property in an object
        /// returns the expected value. Used in the test automation of JSON APIs response
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="finalValue"></param>
        /// <param name="apple"></param>
        /// <returns></returns>
        public static bool checkPath(List<string> paths, string finalValue, dynamic apple)
        {
            if (paths.Count > 1)
            {
                string pathValue = paths[paths.Count - 1];
                int number;
                bool result = Int32.TryParse(pathValue, out number);
                dynamic index = pathValue;
                if (result) { index = number; }

                //keep worming
                if (result)
                {
                    if (apple.Count != 0)
                    {
                        if (apple[index] != null)
                        {
                            paths.RemoveAt(paths.Count - 1);//keep digging                                                            

                            return checkPath(paths, finalValue, apple[index]); ;
                        }
                    }
                }
                else
                {
                    if (apple[index] != null)
                    {
                        paths.RemoveAt(paths.Count - 1);//keep digging                                                            
                        return checkPath(paths, finalValue, apple[index]); ;
                    }
                }
                return false;
            }
            else
            {
                //final check                
                string pathValue = paths[paths.Count - 1];
                int number;
                bool result = Int32.TryParse(pathValue, out number);
                dynamic index = pathValue;
                if (result) { index = number; }
                if (apple[index].ToString() == finalValue)
                {
                    return true;
                }
                return false;
            }
        }
        private static List<Path> CreatePathCollection2(List<string> Chains, Char separator, string TreeName, string Separator2 = "") //universal function
        {
            List<Path> myPaths = new List<Path>();
            foreach (string Link in Chains)
            {
                string[] prePath;
                if (Separator2 == "")
                {
                    prePath = Link.Split(separator);
                }
                else
                {
                    prePath = Link.Split(new string[] { Separator2 }, StringSplitOptions.None);
                }

                List<string> Silabas = new List<string>();

                if (TreeName != "") //if empty dont add father Node
                {
                    Silabas.Add(TreeName); //Optional, some tabs include "sub Interfaces"
                }

                foreach (var item in prePath)
                {
                    Silabas.Add(item);
                }
                Path myPath = new Path
                {
                    Silaba = Silabas
                };
                myPaths.Add(myPath);
            }
            return myPaths;
        }
        private static List<Path> RemoveFirst(List<Path> colPaths)
        {
            List<Path> myPaths = new List<Path>(colPaths);
            foreach (var Path in myPaths)
            {
                Path.Silaba.RemoveAt(0);
            }
            return myPaths;
        }
        private static List<Path> Packager3(List<Path> colPaths)
        {
            if (colPaths.Count > 0)
            {
                List<Path> Paquete = new List<Path>();
                string Silaba = colPaths[0].Silaba[0];

                foreach (var myPath in colPaths) // fix them first
                {
                    if (myPath.Silaba.Count == 0)
                    {
                        myPath.Silaba.Add("RedudantRootNode"); // add a Special Node to detect redundant OL
                    }
                }
                foreach (var myPath in colPaths)
                {
                    if (Silaba == myPath.Silaba[0])
                    {
                        Paquete.Add(myPath);
                    }
                    // some Graph logic going on here...
                    // la logica falla para arboles de sintaxis porque el algoritmo fue hecho
                    // para circuitos explorados por explorer13, por eso falla cuando entra
                    // un path que es un path incompleto, la raiz, sin las hojas. todos los paths deberian llegar
                    // hasta el final, hasta la hoja. Por eso para los Alias no falla, cada Alias es unico
                    // osea llega hasta un hoja
                }
                foreach (var myPath in Paquete)
                {
                    colPaths.Remove(myPath);
                }
                return Paquete;
            }

            return null;
        }
        private static Node CreateTree(List<Path> colPaths)
        {
            List<Path> LocalCopy = new List<Path>();
            List<Path> TrimCollection;
            List<Node> Children = new List<Node>();
            //Node FinalTree = new Node();
            LocalCopy.AddRange(colPaths); //this should copy and not modify the original colPaths, but it looks like this does not works as expeted
            Node Father = new Node
            {
                Trunk = LocalCopy[0].Silaba[0]
            };
            if (LocalCopy.Count > 0 & LocalCopy[0].Silaba.Count > 1)
            {
                TrimCollection = RemoveFirst(LocalCopy);
                Node Intermedio;
                do
                {
                    List<Path> Package = Packager3(TrimCollection);  //testing v2, to see for DRAM_TEST case;                    
                    Intermedio = CreateTree(Package);
                    Children.Add(Intermedio);
                } while (TrimCollection.Count > 0);
            }
            //Father.Branches.AddRange(Children);
            Father.Branches = Children;
            return Father;
        }
        private static Node SimplifyTree2(Node Original, Char WormSeparator, string WormSeparator2 = "")
        {
            List<Node> middleBag = new List<Node>();
            Node Clone = Worm(Original, "", WormSeparator, WormSeparator2);
            if (Clone.Branches != null)
            {
                if (Clone.Branches.Count != 0)
                {
                    foreach (Node Son in Clone.Branches)
                    {
                        if (WormSeparator2 == "")
                        {
                            middleBag.Add(SimplifyTree2(Son, WormSeparator, WormSeparator2));
                        }
                    }
                }
            }
            Node FinalNode = new Node
            {
                Trunk = Clone.Trunk,
                Branches = middleBag
            };
            return FinalNode;
        }
        private static Node Worm(Node Original, string ParentName, char WormSeparator, string WormSeparator2 = "")
        {
            string LocalName;
            if (ParentName == "")
            {
                LocalName = Original.Trunk;
            }
            else
            {
                if (Original.Trunk == ParentName)
                {
                    LocalName = Original.Trunk;
                }
                else
                {
                    if (WormSeparator2 == "")
                    {
                        LocalName = ParentName + WormSeparator + Original.Trunk;   /// Depends, of Alias or PSDF, must change!!!
                    }
                    else
                    {
                        LocalName = ParentName + WormSeparator2 + Original.Trunk;   /// Depends, of Alias or PSDF, must change!!!
                    }
                }
            }
            if (Original.Branches.Count == 1)
            {
                Node Step = new Node();
                Step = Worm(Original.Branches[0], LocalName, WormSeparator, WormSeparator2);
                return Step;
            }
            else
            {
                Node Clone = new Node
                {
                    Trunk = LocalName,
                    Branches = Original.Branches
                };
                return Clone;
            }
        }      
        public static TreeNode TranslateTree(Node AliasTree)
        {
            //parentNode.Text = AliasTree.Trunk;
            TreeNode parentNode = new TreeNode(AliasTree.Trunk);
            List<TreeNode> middleNode = new List<TreeNode>();
            if (AliasTree.Branches.Count != 0)
            {
                foreach (Node myBranch in AliasTree.Branches)
                {
                    middleNode.Add(TranslateTree(myBranch));
                }
            }
            foreach (TreeNode myNode in middleNode)
            {
                parentNode.Nodes.Add(myNode);
            }
            return parentNode;
        }
        #endregion            

        public static TreeNode LoadTreeView7(string FirstName, List<string> myLines, Char Separator, bool Simplify, string Separator2 = "")
        {

            TreeNode FinalTree = new TreeNode(FirstName);
            List<List<string>> myFamilies = PrePackager(myLines, Separator, Separator2);

            if (myFamilies.Count > 1)
            {
                foreach (List<string> subFamily in myFamilies)
                {
                    TreeNode SubTree = LoadGenericTree(subFamily, "", Simplify, Separator, Separator2); //Comment                        
                    FinalTree.Nodes.Add(SubTree);
                }
            }
            else
            {
                FinalTree = LoadGenericTree(myLines, "", Simplify, Separator, Separator2); //Comment
            }

            return FinalTree;
        }
        private static TreeNode LoadGenericTree(List<string> Paths, string TreeName, bool Simplify, char separator, string Separator2 = "")
        {
            List<Path> myPathcol = CreatePathCollection2(Paths, separator, TreeName, Separator2);
            Node Tree;
            Node SimpleTree;
            TreeNode FinalTree = new TreeNode();

            if (Paths.Count != 0)
            {
                Tree = CreateTree(myPathcol); // Create Extendend uncompressed tree            
                if (Simplify)
                {
                    SimpleTree = SimplifyTree2(Tree, separator, Separator2); // Compressed Tree
                }
                else
                {
                    SimpleTree = Tree;
                }
                FinalTree = TranslateTree(SimpleTree);
            }
            else
            {
                FinalTree.Text = "Empty Interface";
            }
            return FinalTree;
        }
        private static Node LoadGenericTree2(List<string> Paths, string TreeName, bool Simplify, char separator, string Separator2 = "")
        {
            List<Path> myPathcol = CreatePathCollection2(Paths, separator, TreeName, Separator2);
            Node Tree;
            Node SimpleTree;
            Node FinalTree = new Node();

            if (Paths.Count != 0)
            {
                Tree = CreateTree(myPathcol); //Create Extendend uncompressed tree            
                if (Simplify)
                {
                    SimpleTree = SimplifyTree2(Tree, separator, Separator2); //Compressed Tree
                }
                else
                {
                    SimpleTree = Tree;
                }
                FinalTree = SimpleTree; //Translate the Node into a TreeNode for TreeView
            }
            else
            {
                FinalTree.Trunk = "EmptyTrunk";
            }
            return FinalTree;
        }
        
        
        private static List<List<string>> PrePackager(List<string> unfilteredPaths, char Separator, string Separator2 = "")
        {
            //pre-grouping the strings into sub-groups to create sub tree
            List<string> SearchList = AuxFunctions.copyList(unfilteredPaths);
            List<string> DuplicatesList = new List<string>();
            List<List<string>> Packagers = new List<List<string>>();

            for (int i = SearchList.Count; i > 0; i--)
            {
                if (!AuxFunctions.Checked(SearchList[i - 1], DuplicatesList))
                {
                    List<string> ShareNameGroup = new List<string>
                    {
                        SearchList[i - 1] //agregar el primero
                    };
                    string firstName = "";
                    firstName = AuxFunctions.FirstName(SearchList[i - 1], Separator, Separator2);
                    SearchList.RemoveAt(i - 1); //Remove from the Top
                    List<string> DuplicateList2 = AuxFunctions.copyList(SearchList);

                    for (int j = DuplicateList2.Count; j > 0; j--)
                    {
                        if (firstName == AuxFunctions.FirstName(DuplicateList2[j - 1], Separator, Separator2))
                        {
                            ShareNameGroup.Add(DuplicateList2[j - 1]);
                            DuplicatesList.Add(DuplicateList2[j - 1]); //not go again through this
                            DuplicateList2.RemoveAt(j - 1);
                        }
                    }
                    Packagers.Add(ShareNameGroup);
                }
            }
            return Packagers;
        }
        public static string OriginalName(TreeNode myNode, char Separator)
        {
            List<string> completeChain = new List<string>();
            List<string> emptyList = new List<string>();
            string completeName = "";
            completeChain = CompleteName2(myNode, emptyList);
            completeChain.Reverse();
            completeName = completeChain[0];
            completeChain.RemoveAt(0);
            foreach (string chain in completeChain)
            {
                completeName = completeName + Separator + chain;
            }
            return completeName;
        }
        private static List<string> CompleteName2(TreeNode node, List<string> NameChain)
        {
            List<string> completeChain = new List<string>();
            List<string> middleChain = new List<string>();
            List<string> LocalChain = NameChain;
            string LocalName = node.Text;
            LocalChain.Add(LocalName);
            if (node.Parent != null)
            {
                middleChain = CompleteName2(node.Parent, LocalChain);
            }
            else
            {
                middleChain = LocalChain;
            }

            completeChain.AddRange(middleChain);
            return completeChain;
        }
        public static Node createJSONTree(string FirstName, List<string> myLines, Char Separator, bool Simplify, string Separator2 = "")
        {
            Node FinalTree = new Node();
            FinalTree.Trunk = FirstName;

            List<List<string>> myFamilies = PrePackager(myLines, Separator, Separator2);

            if (myFamilies.Count > 1)
            {
                foreach (List<string> subFamily in myFamilies)
                {
                    Node SubTree = LoadGenericTree2(subFamily, "", Simplify, Separator, Separator2); //Comment                        
                    FinalTree.Branches.Add(SubTree);
                }
            }
            else
            {
                FinalTree = LoadGenericTree2(myLines, "", Simplify, Separator, Separator2); //Comment
            }

            return FinalTree;
        }
        
        #region Select Tree Functions
        
        public static TreeNode ReturnChecked(TreeNode OriginalTreeNode)
        {
            TreeNode FoundTreeNode = new TreeNode(); //Asumo que solo hay un check! solo devolvera el ultimo que encuentre
            bool Found = false;
            int index = 0;

            if (!OriginalTreeNode.Checked)
            {
                if (OriginalTreeNode.Nodes.Count != 0)
                {
                    while (!Found & index < OriginalTreeNode.Nodes.Count) //ignore the Count-1, it has to do it one last time
                    {
                        if (OriginalTreeNode.Nodes[index].Checked)
                        {
                            FoundTreeNode = OriginalTreeNode.Nodes[index];

                            Found = true;
                        }
                        else
                        {
                            FoundTreeNode = ReturnChecked(OriginalTreeNode.Nodes[index]);

                            if (FoundTreeNode != null)
                            {
                                Found = true;
                            }
                        }
                        index++;
                    }
                }
                else
                {
                    return null;
                }
                return FoundTreeNode;
            }
            else
            {
                return OriginalTreeNode;
            }
        }
        public static List<TreeNode> ReturnLastGeneration(TreeNode OriginalTreeNode)
        {
            List<TreeNode> myLastGeneration = new List<TreeNode>();

            if (!(OriginalTreeNode.Nodes.Count == 0))
            {
                List<TreeNode> middleBag = new List<TreeNode>();

                foreach (TreeNode treeNode in OriginalTreeNode.Nodes)
                {
                    if (treeNode.Nodes.Count == 0) //no tengo hijos, soy el ultimo
                    {
                        myLastGeneration.Add(treeNode);
                    }
                    else
                    {
                        middleBag = ReturnLastGeneration(treeNode);
                    }
                }

                myLastGeneration.AddRange(middleBag);
                return myLastGeneration;
            }
            else
            {
                myLastGeneration.Add(OriginalTreeNode);
                return myLastGeneration;
            }
        }
        public static List<TreeNode> ReturnLastGeneration2(TreeNode OriginalTreeNode)
        {
            List<TreeNode> myLastGeneration = new List<TreeNode>();

            if (!(OriginalTreeNode.Nodes.Count == 0))
            {
                List<TreeNode> middleBag = new List<TreeNode>();

                foreach (TreeNode treeNode in OriginalTreeNode.Nodes)
                {
                    if (treeNode.Nodes.Count == 0) //no tengo hijos, soy el ultimo
                    {
                        myLastGeneration.Add(treeNode);
                    }
                    else
                    {
                        middleBag.AddRange(ReturnLastGeneration2(treeNode));  //Change needed to AddRange
                    }
                }

                myLastGeneration.AddRange(middleBag);
                return myLastGeneration;
            }
            else
            {
                myLastGeneration.Add(OriginalTreeNode);
                return myLastGeneration;
            }
        }
        #endregion

        #region Async Versions

        /// <summary>
        /// Just a wrapper to create tasks
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="myLines"></param>
        /// <param name="Separator"></param>
        /// <param name="Simplify"></param>
        /// <param name="Separator2"></param>
        /// <returns></returns>
        public static Task<Node> createJSONTreeAsync(string FirstName, List<string> myLines, Char Separator, bool Simplify, string Separator2 = "")
        {
            return Task.FromResult(createJSONTree(FirstName, myLines, Separator, Simplify, Separator2));
        }

        #endregion
        private static void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked) //taken from Platform Maker
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }
        private static List<TreeNode> ReturnChecked2(TreeNode ParentNode) //taken from Platform Maker
        {
            List<TreeNode> LittleBag = new List<TreeNode>();
            List<TreeNode> MiddleBag = new List<TreeNode>();
            List<TreeNode> LargeBag = new List<TreeNode>();
            if (ParentNode.Nodes.Count != 0)
            {
                foreach (TreeNode ChildNode in ParentNode.Nodes)
                {
                    if (ChildNode.Checked)
                    {
                        LittleBag.Add(ChildNode);
                    }
                    else
                    {
                        MiddleBag.AddRange(ReturnChecked2(ChildNode));
                    }
                }
            }
            MiddleBag.AddRange(LittleBag);
            LargeBag.AddRange(MiddleBag);
            return LargeBag;
        }
    }
}