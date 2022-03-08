using UnityEngine;
using System.Collections;
using System;//
//堆栈，完全二叉树中：
//parent：(n-1)/2
//left_child :2n+1
//right_child:2n+2
//节点向上走，只需要和父节点比较就好
//节点向下走，需要和两个孩子比较
//where关键字说明泛型T的由来
public class Heap<T> where T : IHeapItem<T> {
	//items这是一个数组叭,泛型编程
	T[] items;
	int currentItemCount;
	//初始化堆
	//从一开始就要知道堆的最大大小，因为数组很难确定
	public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}
	//新元素添加在最后面
	public void Add(T item) {
		//为新元素添加堆索引
		item.HeapIndex = currentItemCount;
		//将新元素添加在数组末尾
		items[currentItemCount] = item;
		//重新对堆进行排序，向上排序
		SortUp(item);
		//当前数组数量加一
		currentItemCount++;
	}

	public T RemoveFirst() {
		//取最小值
		T firstItem = items[0];
		//数量减一
		currentItemCount--;
		//将最后一个元素放在堆的最上面
		items[0] = items[currentItemCount];
		//堆序号和数组下标是不一样的
		items[0].HeapIndex = 0;
		//再次构造小顶堆，向下构造
		SortDown(items[0]);
		return firstItem;
	}
	//这个函数是干么的
	//这个函数是为了更新邻居的距离
	//一旦调用这个韩式就是要增加邻居节点的优先级，所以不需要排序
	public void UpdateItem(T item) {
		SortUp(item);
	}
	//当前数组长度，数组下标加一
	public int Count {
		get {
			return currentItemCount;
		}
	}
	
	//这个函数是用来检查这个堆中是否包含特定的项
	//可是这个函数也没有遍历啊
	public bool Contains(T item) {
		//这个堆是用在open表中搜索f值最小的项
		return Equals(items[item.HeapIndex], item);
	}
	//向下构造，就是父节点与孩子们比较
	void SortDown(T item) {
		while (true) {
			//左孩子的堆中索引
			int childIndexLeft = item.HeapIndex * 2 + 1;
			//右孩子的堆中索引
			int childIndexRight = item.HeapIndex * 2 + 2;
			//这个变量是干嘛呢
			int swapIndex = 0;
			//左孩子没越界
			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;
				//右孩子没越界
				if (childIndexRight < currentItemCount) {
					//s1.CompareTo(s2)
					//当s1>s2时，s1.CompareTo(s2)=1
					//当s1=s2时，s1.CompareTo(s2)=0
					//当s1<s2时，s1.CompareTo(s2)=-1
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						//右孩子小
						swapIndex = childIndexRight;
					}
				}
				//孩子更小，那就要与父节点交换
				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap (item,items[swapIndex]);
				}
				else {
					return;
				}

			}
			else {
				return;
			}

		}
	}
	
	void SortUp(T item) {
		//向上构建，只需要和父节点进行比较就好啦
		int parentIndex = (item.HeapIndex-1)/2;
		
		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				 Swap (item,parentItem);
			}
			else {
				break;
			}

			parentIndex = (item.HeapIndex-1)/2;
		}
	}
	
	void Swap(T itemA, T itemB) {
		//不仅要交换数组中的位置，还要交换堆中的索引
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		//交换堆索引值
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}
//当我们自己在创建一个结构体想要提升它的比较效率的时候，我们自己也能通过IComparable来实现
//感觉是为了使用compareTo函数，引入的接口
//泛型编程，IHeaoItem 继承自IConpareble,需要引入头文件system
public interface IHeapItem<T> : IComparable<T> {
	//HeapIndex，堆索引的由来
	//这里不是很懂
	//
	int HeapIndex {
		get;
		set;
	}
}
