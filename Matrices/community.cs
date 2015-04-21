////////////////////////////////////////////////////////////////////////
// --- COPYRIGHT NOTICE ---------------------------------------------
// FastCommunityMH - infers community structure of networks
// Copyright (C) 2004 Aaron Clauset
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// 
// See http://www.gnu.org/licenses/gpl.txt for more details.
// 
////////////////////////////////////////////////////////////////////////
// Author       : Aaron Clauset  (aaron@cs.unm.edu)				//
// Location     : U. Michigan, U. New Mexico						//
// Time         : January-August 2004							//
// Collaborators: Dr. Cris Moore (moore@cs.unm.edu)				//
//              : Dr. Mark Newman (mejn@umich.edu)				//
////////////////////////////////////////////////////////////////////////
// --- DEEPER DESCRIPTION ---------------------------------------------
//  see http://www.arxiv.org/abs/cond-mat/0408187 for more information
// 
//  - read network structure from data file (see below for constraints)
//  - builds dQ, H and a data structures
//  - runs new fast community structure inference algorithm
//  - records Q(t) function to file
//  - (optional) records community structure (at t==cutstep)
//  - (optional) records the list of members in each community (at t==cutstep)
//
////////////////////////////////////////////////////////////////////////
// --- PROGRAM USAGE NOTES ---------------------------------------------
// This program is rather complicated and requires a specific kind of input,
// so some notes on how to use it are in order. Mainly, the program requires
// a specific structure input file (.pairs) that has the following characteristics:
//  
//  1. .pairs is a list of tab-delimited pairs of numeric indices, e.g.,
//		"54\t91\n"
//  2. the network described is a SINGLE COMPONENT
//  3. there are NO SELF-LOOPS or MULTI-EDGES in the file; you can use
//     the 'netstats' utility to extract the giantcomponent (-gcomp.pairs)
//     and then use that file as input to this program
//  4. the MINIMUM NODE ID = 0 in the input file; the maximum can be
//     anything (the program will infer it from the input file)
// 
// Description of commandline arguments
// -f <filename>    give the target .pairs file to be processed
// -l <text>		the text label for this run; used to build output filenames
// -t <int>		timer period for reporting progress of file input to screen
// -s			calculate and record the support of the dQ matrix
// -v --v ---v		differing levels of screen output verbosity
// -o <directory>   directory for file output
// -c <int>		record the aglomerated network at step <int>
// 
////////////////////////////////////////////////////////////////////////
// Change Log:
// 2006-02-06: 1) modified readInputFile to be more descriptive of its actions
//             2) removed -o functionality; program writes output to directory
//             of input file. (also removed -h option of command line)
// 2006-10-13: 3) Janne Aukia (jaukia@cc.hut.fi) suggested changes to the 
//             mergeCommunities() function here (see comments in that function),
//             and an indexing adjustment in printHeapTop10() in maxheap.h.
//
////////////////////////////////////////////////////////////////////////

//Adapted by Peter Marteen

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using CH.Combinations;
using Network.Matrices;
using Network.IO;
using Network.Cliques;
using modmax;
using Vektor;

namespace Community
{
    // ------------------------------------------------------------------------------------
    // Edge object - defined by a pair of vertex indices and *edge pointer to next in linked-list
    public class edge
    {
        public int so;					// originating node
        public int si;					// terminating node
        public edge next;					// pointer for linked list of edges

        public edge() { so = 0; si = 0; next = null; }						// default constructor
        ~edge() { }
    };


    // ------------------------------------------------------------------------------------
    // Nodenub object - defined by a *node pointer and *node pointer 
    public struct nodenub
    {
        public tuple heap_ptr;			// pointer to node(max,i,j) in max-heap of row maxes
        public vektor v;					// pointer stored vector for (i,j)
    };

    // ------------------------------------------------------------------------------------
    // tuple object - defined by an real value and (row,col) indices
    //public struct tuple
    //{
    //    public double m;					// stored value
    //    public int i;					// row index
    //    public int j;					// column index
    //    public int k;					// heap index
    //    public bool enabled = false;
    //};

    // ordered pair structures (handy in the program)
    public struct apair
    {
        public int x;
        public int y;
    };

    public struct returnValues
    {
        public nodenub[] communities;
        public double[] Q;
        public apair[] join;
    };

    //public class dpair
    //{
    //    public int x;
    //    public double y;
    //    public dpair next;
    //    public dpair()
    //    {
    //        x = 0; y = 0.0; next = null;
    //    }
    //    ~dpair() { }
    //};



    // ------------------------------------------------------------------------------------
    // List object - simple linked list of integers
    public class list
    {
        public int index;				// node index
        public list next;				// pointer to next element in linked list
        public list() { index = 0; next = null; }
        ~list() { }
    };

    // ------------------------------------------------------------------------------------
    // Community stub object - stub for a community list
    public class stub
    {
        public bool valid;				// is this community valid?
        public int size;				// size of community
        public list members;				// pointer to list of community members
        public list last;				// pointer to end of list

        public stub() { valid = false; size = 0; members = null; last = null; }

    };


    public class Community
    {
        // ------------------------------------------------------------------------------------
        // FUNCTION DECLARATIONS --------------------------------------------------------------



        // ------------------------------------------------------------------------------------
        // PROGRAM PARAMETERS -----------------------------------------------------------------

        public struct netparameters
        {
            public int n;				// number of nodes in network
            public int m;				// number of edges in network
            public int maxid;			// maximum node id
            public int minid;			// minimum node id
        }; netparameters gparm;



        returnValues values;


        // ------------------------------------------------------------------------------------
        // ----------------------------------- GLOBAL VARIABLES -------------------------------

        public char pauseme;
        public edge[] e;				// initial adjacency matrix (sparse)
        public edge[] elist;			// list of edges for building adjacency matrix
        public nodenub[] dq;				// dQ matrix
        public modmaxheap h;				// heap of values from max_i{dQ_ij}
        public double[] Q;				// Q(t)
        public dpair Qmax;			// maximum Q value and the corresponding time t
        public double[] a;				// A_i
        public apair[] joins;			// list of joins
        public stub[] c;				// link-lists for communities

        enum comm { NONE };

        int supportTot;
        double supportAve;

        // ------------------------------------------------------------------------------------
        // ----------------------------------- MAIN PROGRAM -----------------------------------
        public returnValues calculate(edge[] ein, edge[]einlist, int max, int numL, int numN)
        {
            Qmax = new dpair();
            e = new edge[max];
            elist = new edge[2 * numN];
            e = ein;
            elist = einlist;
            gparm.m = numL;
            gparm.n = numN;
            gparm.maxid = max+1;
            
            // ----------------------------------------------------------------------



            // ----------------------------------------------------------------------
            // Allocate data structures for main loop
            a = new double[gparm.maxid];
            Q = new double[gparm.n + 1];
            joins = new apair[gparm.n + 1];
            //h = new Maxheap<tuple>();
            c = new stub[gparm.n + 1];
           


            int t = 1;
            Qmax.y = -4294967296.0;
            

            buildDeltaQMatrix();							// builds dQ[] and h




            // ----------------------------------------------------------------------
            // Start FastCommunity algorithm

            tuple dQmax;
            int isupport, jsupport;
            while (h.heapSize() > 2)
            {

                // ---------------------------------
                // Find largest dQ

                dQmax = h.popMaximum();					// select maximum dQ_ij // convention: insert i into j
                

                if (dQmax.m < -4000000000.0) { break; }		// no more joins possible


                // ---------------------------------
                // Merge the chosen communities

                //if (dq[dQmax.i].v == null || dq[dQmax.j].v == null)
                //{
                //    throw new Exception("Error");
                //}
                if (dq[dQmax.i].v != null && dq[dQmax.j].v != null)
                {
                    isupport = dq[dQmax.i].v.returnNodecount();
                    jsupport = dq[dQmax.j].v.returnNodecount();
                    if (isupport < jsupport)
                    {
                        mergeCommunities(dQmax.i, dQmax.j);	// merge community i into community j
                        joins[t].x = dQmax.i;				// record merge of i(x) into j(y)
                        joins[t].y = dQmax.j;				// 
                    }
                    else
                    {								// 
                        dq[dQmax.i].heap_ptr = dq[dQmax.j].heap_ptr; // take community j's heap pointer
                        dq[dQmax.i].heap_ptr.i = dQmax.i;			//   mark it as i's
                        dq[dQmax.i].heap_ptr.j = dQmax.j;			//   mark it as i's
                        mergeCommunities(dQmax.j, dQmax.i);	// merge community j into community i
                        joins[t].x = dQmax.j;				// record merge of j(x) into i(y)
                        joins[t].y = dQmax.i;				// 
                    }									// 
                    Q[t] = dQmax.m + Q[t - 1];					// record Q(t)

                    // ---------------------------------
                    // Record join to file
                    // Note that it is the .joins file which contains both the dendrogram and the corresponding
                    // Q values. The file format is tab-delimited columns of data, where the columns are:
                    // 1. the community which grows
                    // 2. the community which was absorbed
                    // 3. the modularity value Q after the join
                    // 4. the time step value

                    // ---------------------------------
                    // If cutstep valid, then do some work
                    //if (t <= ioparm.cutstep) { groupListsUpdate(joins[t].x, joins[t].y); }
                    //if (t == ioparm.cutstep) { recordNetwork(); recordGroupLists(); groupListsStats(); }

                    // ---------------------------------
                    // Record the support data to file

                    if (Q[t] > Qmax.y) { Qmax.y = Q[t]; Qmax.x = t; }

                    t++;									// increment time
                }
            } // ------------- end community merging loop

            values.Q = a;
            values.communities = dq;
            values.join = joins;
            return values;
        }


        // ------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------ //
        // FUNCTION DEFINITIONS --------------------------------------------------------------- //

        void buildDeltaQMatrix()
        {

            // Given that we've now populated a sparse (unordered) adjacency matrix e (e), 
            // we now need to construct the intial dQ matrix according to the definition of dQ
            // which may be derived from the definition of modularity Q:
            //    Q(t) = \sum_{i} (e_{ii} - a_{i}^2) = Tr(e) - ||e^2||
            // thus dQ is
            //    dQ_{i,j} = 2* ( e_{i,j} - a_{i}a_{j} )
            //    where a_{i} = \sum_{j} e_{i,j} (i.e., the sum over the ith row)
            // To create dQ, we must insert each value of dQ_{i,j} into a binary search tree,
            // for the jth column. That is, dQ is simply an array of such binary search trees,
            // each of which represents the dQ_{x,j} adjacency vector. Having created dQ as
            // such, we may happily delete the matrix e in order to free up more memory.
            // The next step is to create a max-heap data structure, which contains the entries
            // of the following form (value, s, t), where the heap-key is 'value'. Accessing the
            // root of the heap gives us the next dQ value, and the indices (s,t) of the vectors
            // in dQ which need to be updated as a result of the merge.


            // First we compute e_{i,j}, and the compute+store the a_{i} values. These will be used
            // shortly when we compute each dQ_{i,j}.
            edge current;

            
            double eij = (double)(0.5 / gparm.m);				// intially each e_{i,j} = 1/m
            for (int i = 1; i < gparm.maxid; i++)
            {				// for each row
   						 
                if (e[i].so != 0)
                {							//    ensure it exists
                    current = e[i];						//    grab first edge
                    a[i] = eij;							//    initialize a[i]
                    while (current.next != null)
                    {			//    loop through remaining edges
                        a[i] += eij;						//       add another eij
                        current = current.next;				//
                    }
                    Q[0] += -1.0 * a[i] * a[i];					// calculate initial value of Q
                }
            }

            // now we create an empty (ordered) sparse matrix dq[]
            dq = new nodenub[gparm.maxid];						// initialize dq matrix
            for (int i = 0; i < gparm.maxid; i++)
            {					// 
                //dq[i].heap_ptr;							// no pointer in the heap at first
                if (e[i].so != 0) { dq[i].v = new vektor(2 + (int)(gparm.m * a[i])); }
                else { dq[i].v = null; }
            }
            h = new modmaxheap(gparm.n);						// allocate max-heap of size = number of nodes

            // Now we do all the work, which happens as we compute and insert each dQ_{i,j} into 
            // the corresponding (ordered) sparse vector dq[i]. While computing each dQ for a
            // row i, we track the maximum dQmax of the row and its (row,col) indices (i,j). Upon
            // finishing all dQ's for a row, we insert the tuple into the max-heap hQmax. That
            // insertion returns the itemaddress, which we then store in the nodenub heap_ptr for 
            // that row's vector.
            double dQ;
            tuple dQmax = new tuple();										// for heaping the row maxes
            //tuple itemaddress;
            // stores address of item in maxheap


            for (int i = 1; i < gparm.maxid; i++)
            {
                if (e[i].so != 0)
                {
                    current = e[i];								// grab first edge
                    dQ = 2.0 * (eij - (a[current.so] * a[current.si]));   // compute its dQ
                    dQmax.m = dQ;									// assume it is maximum so far
                    dQmax.i = current.so;							// store its (row,col)
                    dQmax.j = current.si;							//
                    dQmax.enabled = true;
                    dq[i].v.insertItem(current.si, dQ);				// insert its dQ
                    while (current.next != null)
                    {					// 
                        current = current.next;						// step to next edge
                        dQ = 2.0 * (eij - (a[current.so] * a[current.si]));	// compute new dQ
                        if (dQ > dQmax.m)
                        {							// if dQ larger than current max
                            dQmax.m = dQ;							//    replace it as maximum so far
                            dQmax.j = current.si;					//    and store its (col)
                        }
                        dq[i].v.insertItem(current.si, dQ);			// insert it into vector[i]
                    }
                    dq[i].heap_ptr = h.insertItem(dQmax);				// store the pointer to its loc in heap
                }
            }

            e = null;
            elist = null;

            // free-up adjacency matrix memory in two shots
            // 
            return;
        }

        // ------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------ //

        // ------------------------------------------------------------------------------------
        // returns the support of the dQ[]

        void dqSupport()
        {
            int total = 0;
            int count = 0;
            for (int i = 0; i < gparm.maxid; i++)
            {
                if (dq[i].heap_ptr.enabled == false) { total += dq[i].v.returnNodecount(); count++; }
            }
            supportTot = total;
            supportAve = total / (double)count;
            return;
        }

        // ------------------------------------------------------------------------------------

        void groupListsSetup()
        {

            list newList;
            c = new stub[gparm.maxid];
            for (int i = 0; i < gparm.maxid; i++)
            {
                if (e[i].so != 0)
                {								// note: internal indexing
                    newList = new list();							// create new community member
                    newList.index = i;							//    with index i
                    c[i].members = newList;					// point ith community at newList
                    c[i].size = 1;							// point ith community at newList
                    c[i].last = newList;					// point last[] at that element too
                    c[i].valid = true;						// mark as valid community
                }
            }

            return;
        }


        void mergeCommunities(int i, int j)
        {

            // To do the join operation for a pair of communities (i,j), we must update the dQ
            // values which correspond to any neighbor of either i or j to reflect the change.
            // In doing this, there are three update rules (cases) to follow:
            //  1. jix-triangle, in which the community x is a neighbor of both i and j
            //  2. jix-chain, in which community x is a neighbor of i but not j
            //  3. ijx-chain, in which community x is a neighbor of j but not i
            //
            // For the first two cases, we may make these updates by simply getting a list of
            // the elements (x,dQ) of [i] and stepping through them. If x==j, then we can ignore
            // that value since it corresponds to an edge which is being absorbed by the joined
            // community (i,j). If [j] also contains an element (x,dQ), then we have a triangle;
            // if it does not, then we have a jix-chain.
            //
            // The last case requires that we step through the elements (x,dQ) of [j] and update each
            // if [i] does not have an element (x,dQ), since that implies a ijx-chain.
            // 
            // Let d([i]) be the degree of the vector [i], and let k = d([i]) + d([j]). The running
            // time of this operation is O(k log k)
            //
            // Essentially, we do most of the following operations for each element of
            // dq[i]_x where x \not= j
            //  1.  add dq[i]_x to dq[j]_x (2 cases)
            //  2.  remove dq[x]_i
            //  3.  update maxheap[x]
            //  4.  add dq[i]_x to dq[x]_j (2 cases)
            //  5.  remove dq[j]_i
            //  6.  update maxheap[j]
            //  7.  update a[j] and a[i]
            //  8.  delete dq[i]

            dpair list, current, temp;
            tuple newMax;
            int t = 1;

            // -- Working with the community being inserted (dq[i])
            // The first thing we must do is get a list of the elements (x,dQ) in dq[i]. With this 
            // list, we can then insert each into dq[j].

            //	dq[i].v->printTree();
            list = dq[i].v.returnTreeAsList();			// get a list of items in dq[i].v
            current = list;							// store ptr to head of list



            // ---------------------------------------------------------------------------------
            // SEARCHING FOR JIX-TRIANGLES AND JIX-CHAINS --------------------------------------
            // Now that we have a list of the elements of [i], we can step through them to check if
            // they correspond to an jix-triangle, a jix-chain, or the edge (i,j), and do the appropriate
            // operation depending.

            while (current != null)
            {						// insert list elements appropriately


                // If the element (x,dQ) is actually (j,dQ), then we can ignore it, since it will 
                // correspond to an edge internal to the joined community (i,j) after the join.
                if (current.x != j)
                {

                    // Now we must decide if we have a jix-triangle or a jix-chain by asking if
                    // [j] contains some element (x,dQ). If the following conditional is TRUE,
                    // then we have a jix-triangle, ELSE it is a jix-chain.

                    if (dq[j].v.findItem(current.x) != null)
                    {
                        // CASE OF JIX-TRIANGLE

                        // We first add (x,dQ) from [i] to [x] as (j,dQ), since [x] essentially now has
                        // two connections to the joined community [j].

                        dq[current.x].v.insertItem(j, current.y);			// (step 1)

                        // Then we need to delete the element (i,dQ) in [x], since [i] is now a
                        // part of [j] and [x] must reflect this connectivity.

                        dq[current.x].v.deleteItem(i);					// (step 2)

                        // After deleting an item, the tree may now have a new maximum element in [x],
                        // so we need to check it against the old maximum element. If it's new, then
                        // we need to update that value in the heap and reheapify.
                        newMax = dq[current.x].v.returnMaxStored();		// (step 3)

                        //				if (newMax.m > dq[current->x].heap_ptr->m || dq[current->x].heap_ptr->j==i) {
                        h.updateItem(dq[current.x].heap_ptr, newMax);

                        //				}
                        // Change suggested by Janne Aukia (jaukia@cc.hut.fi) on 12 Oct 2006

                        // Finally, we must insert (x,dQ) into [j] to note that [j] essentially now
                        // has two connections with its neighbor [x].

                        dq[j].v.insertItem(current.x, current.y);		// (step 4)

                    }
                    else
                    {
                        // CASE OF JIX-CHAIN

                        // The first thing we need to do is calculate the adjustment factor (+) for updating elements.
                        double axaj = -2.0 * a[current.x] * a[j];

                        // Then we insert a new element (j,dQ+) of [x] to represent that [x] has
                        // acquired a connection to [j], which was [x]'d old connection to [i]
                        dq[current.x].v.insertItem(j, current.y + axaj);	// (step 1)

                        // Now the deletion of the connection from [x] to [i], since [i] is now
                        // a part of [j]

                        dq[current.x].v.deleteItem(i);					// (step 2)

                        // Deleting that element may have changed the maximum element for [x], so we
                        // need to check if the maximum of [x] is new (checking it against the value
                        // in the heap) and then update the maximum in the heap if necessary.
                        newMax = dq[current.x].v.returnMaxStored();		// (step 3)

                        //				if (newMax.m > dq[current->x].heap_ptr->m || dq[current->x].heap_ptr->j==i) {
                        h.updateItem(dq[current.x].heap_ptr, newMax);

                        //				}
                        // Change suggested by Janne Aukia (jaukia@cc.hut.fi) on 12 Oct 2006

                        // Finally, we insert a new element (x,dQ+) of [j] to represent [j]'s new
                        // connection to [x]

                        dq[j].v.insertItem(current.x, current.y + axaj);	// (step 4)

                    }    // if (dq[j].v->findItem(current->x))

                }    // if (current->x != j)

                temp = current;
                current = current.next;						// move to next element

                temp = null;
                t++;
            }    // while (current!=NULL)

            // We've now finished going through all of [i]'s connections, so we need to delete the element
            // of [j] which represented the connection to [i]


            dq[j].v.deleteItem(i);						// (step 5)

            // We can be fairly certain that the maximum element of [j] was also the maximum
            // element of [i], so we need to check to update the maximum value of [j] that
            // is in the heap.
            newMax = dq[j].v.returnMaxStored();			// (step 6)

            h.updateItem(dq[j].heap_ptr, newMax);


            // ---------------------------------------------------------------------------------
            // SEARCHING FOR IJX-CHAINS --------------------------------------------------------
            // So far, we've treated all of [i]'s previous connections, and updated the elements
            // of dQ[] which corresponded to neighbors of [i] (which may also have been neighbors
            // of [j]. Now we need to update the neighbors of [j] (as necessary)

            // Again, the first thing we do is get a list of the elements of [j], so that we may
            // step through them and determine if that element constitutes an ijx-chain which
            // would require some action on our part.
            list = dq[j].v.returnTreeAsList();			// get a list of items in dq[j].v
            current = list;							// store ptr to head of list
            t = 1;


            while (current != null)
            {					// insert list elements appropriately


                // If the element (x,dQ) of [j] is not also (i,dQ) (which it shouldn't be since we've
                // already deleted it previously in this function), and [i] does not also have an
                // element (x,dQ), then we have an ijx-chain.
                if ((current.x != i) && (dq[i].v.findItem(current.x) != null))
                {
                    // CASE OF IJX-CHAIN

                    // First we must calculate the adjustment factor (+).
                    double axai = -2.0 * a[current.x] * a[i];

                    // Now we must add an element (j,+) to [x], since [x] has essentially now acquired
                    // a new connection to [i] (via [j] absorbing [i]).
                    dq[current.x].v.insertItem(j, axai);			// (step 1)

                    // This new item may have changed the maximum dQ of [x], so we must update it.
                    newMax = dq[current.x].v.returnMaxStored();	// (step 3)

                    h.updateItem(dq[current.x].heap_ptr, newMax);

                    // And we must add an element (x,+) to [j], since [j] as acquired a new connection
                    // to [x] (via absorbing [i]).
                    dq[j].v.insertItem(current.x, axai);			// (step 4)
                    newMax = dq[j].v.returnMaxStored();			// (step 6)

                    h.updateItem(dq[j].heap_ptr, newMax);


                }    //  (current->x != i && !dq[i].v->findItem(current->x))

                temp = current;
                current = current.next;						// move to next element

                temp = null;
                t++;
            }    // while (current!=NULL)

            // Now that we've updated the connections values for all of [i]'s and [j]'s neighbors, 
            // we need to update the a[] vector to reflect the change in fractions of edges after
            // the join operation.

            a[j] += a[i];								// (step 7)
            a[i] = 0.0;

            // ---------------------------------------------------------------------------------
            // Finally, now we need to clean up by deleting the vector [i] since we'll never
            // need it again, and it'll conserve memory. For safety, we also set the pointers
            // to be NULL to prevent inadvertent access to the deleted data later on.

            // (step 8)
            dq[i].v = null;						// (step 8)
            dq[i].heap_ptr.enabled = false;						//

            return;

        }

        // ------------------------------------------------------------------------------------


        // ------------------------------------------------------------------------------------

        public void Initialize(int numnodes, int numlinks)
        {

            //// temporary variables for this function
            //int numnodes = 0;
            //int numlinks = 0;
            //int s, f, t;
            edge[] last;



            //// First scan through the input file to discover the largest node id. We need to
            //// do this so that we can allocate a properly sized array for the sparse matrix
            //// representation.


            //while (fscan >> s >> f)
            //{					// read friendship pair (s,f)
            //    numlinks++;							// count number of edges
            //    if (f < s) { t = s; s = f; f = t; }		// guarantee s < f
            //    if (f > numnodes) { numnodes = f; }		// track largest node index

            //    if (t2 - t1 > ioparm.timer)
            //    {				// check timer; if necessarsy, display

            //        t1 = t2;							// 
            //        ioparm.timerFlag = true;				// 
            //    }									// 
            //    t2 = time(&t2);							// 

            //}
            //fscan.close();

            gparm.maxid = numnodes + 2;					// store maximum index
            elist = new edge[2 * numlinks];				// create requisite number of edges
  					// index of next edge of elist to be used

            //// Now that we know numnodes, we can allocate the space for the sparse matrix, and
            //// then reparse the file, adding edges as necessary.

            e = new edge[gparm.maxid];			// (unordered) sparse adjacency matrix
            last = new edge[gparm.maxid];			// list of pointers to the last edge in each row
            numnodes = 0;								// numnodes now counts number of actual used node ids
            numlinks = 0;								// numlinks now counts number of bi-directional edges created
            //ioparm.timerFlag = false;					// reset timer



            //while (fin >> s >> f)
            //{
            //    s++; f++;								// increment s,f to prevent using e[0]
            //    if (f < s) { t = s; s = f; f = t; }		// guarantee s < f
            //    numlinks++;							// increment link count (preemptive)
            //    if (e[s].so == 0)
            //    {						// if first edge with s, add s and (s,f)
            //        e[s].so = s;						// 
            //        e[s].si = f;						// 
            //        last[s] = &e[s];					//    point last[s] at self
            //        numnodes++;						//    increment node count
            //    }
            //    else
            //    {								//    try to add (s,f) to s-edgelist
            //        current = &e[s];					// 
            //        existsFlag = false;					// 
            //        while (current != NULL)
            //        {			// check if (s,f) already in edgelist
            //            if (current->si == f)
            //            {			// 
            //                existsFlag = true;			//    link already exists
            //                numlinks--;				//    adjust link-count downward
            //                break;					// 
            //            }							// 
            //            current = current->next;			//    look at next edge
            //        }								// 
            //        if (!existsFlag)
            //        {					// if not already exists, append it
            //            newedge = &elist[ecounter++];		//    grab next-free-edge
            //            newedge->so = s;				// 
            //            newedge->si = f;				// 
            //            last[s]->next = newedge;		//    append newedge to [s]'s list
            //            last[s] = newedge;		//    point last[s] to newedge
            //        }								// 
            //    }									// 

            //    if (e[f].so == 0)
            //    {						// if first edge with f, add f and (f,s)
            //        e[f].so = f;						// 
            //        e[f].si = s;						// 
            //        last[f] = &e[f];					//    point last[s] at self
            //        numnodes++;						//    increment node count
            //    }
            //    else
            //    {								// try to add (f,s) to f-edgelist
            //        if (!existsFlag)
            //        {					//    if (s,f) wasn't in s-edgelist, then
            //            newedge = &elist[ecounter++];		//       (f,s) not in f-edgelist
            //            newedge->so = f;				// 
            //            newedge->si = s;				// 
            //            last[f]->next = newedge;		//    append newedge to [f]'s list
            //            last[f] = newedge;		//    point last[f] to newedge
            //        }								// 
            //    }
            //    existsFlag = false;						// reset existsFlag

            //}




            //gparm.m = numlinks;							// store actual number of edges created
            //gparm.n = numnodes;							// store actual number of nodes used
            return;
        }

        void groupListsUpdate(int x, int y)
        {

            c[y].last.next = c[x].members;				// attach c[y] to end of c[x]
            c[y].last = c[x].last;					// update last[] for community y
            c[y].size += c[x].size;					// add size of x to size of y

            c[x].members = null;						// delete community[x]
            c[x].valid = false;						// 
            c[x].size = 0;							// 
            c[x].last = null;						// delete last[] for community x

            return;
        }


        // ------------------------------------------------------------------------------------
        // records the agglomerated list of indices for each valid community 

        //void recordGroupLists() {

        //    list *current;
        //    ofstream fgroup(ioparm.f_group.c_str(), ios::trunc);
        //    for (int i=0; i<gparm.maxid; i++) {
        //        if (c[i].valid) {
        //            fgroup << "GROUP[ "<<i-1<<" ][ "<<c[i].size<<" ]\n";   // external format
        //            current = c[i].members;
        //            while (current != NULL) {
        //                fgroup << current->index-1 << "\n";			// external format
        //                current = current->next;				
        //                }
        //        }
        //    }
        //    fgroup.close();

        //    return;
        //}

        // ------------------------------------------------------------------------------------
        // records the network as currently agglomerated

        //void recordNetwork() {

        //    dpair *list, *current, *temp;

        //    ofstream fnet(ioparm.f_net.c_str(), ios::trunc);
        //    for (int i=0; i<gparm.maxid; i++) {
        //        if (dq[i].heap_ptr != NULL) {
        //            list    = dq[i].v->returnTreeAsList();			// get a list of items in dq[i].v
        //            current = list;							// store ptr to head of list
        //            while (current != NULL) {
        //                //		source		target		weight    (external representation)
        //                fnet << i-1 << "\t" << current->x-1 << "\t" << current->y << "\n";

        //                temp = current;						// clean up memory and move to next
        //                current = current->next;
        //                delete temp;				
        //            }
        //        }		
        //    }
        //    fnet.close();

        //    return;
        //}
    }
}
// ------------------------------------------------------------------------------------
// ------------------------------------------------------------------------------------




