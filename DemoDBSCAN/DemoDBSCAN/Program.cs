using System;
using System.Collections.Generic;
using System.Linq;

class Point
{
	public const int NOISE = -1; //điểm nhiễu
	public const int chuaphanloai = 0;
	public int X, Y, CumId;
	public Point(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}
	public override string ToString()
	{
		return String.Format("({0}, {1})", X, Y);
	}
	public static int khoangcach(Point p1, Point p2)
	{
		int diffX = p2.X - p1.X;
		int diffY = p2.Y - p1.Y;
		return diffX * diffX + diffY * diffY;
	}
}
static class DBSCAN
{
	static void Main()
	{
		List<Point> points = new List<Point>();
        // Khởi tạo dữ liệu
        points.Add(new Point(0, 100));
        points.Add(new Point(0, 200));
        points.Add(new Point(0, 275));
        points.Add(new Point(100, 150));
        points.Add(new Point(200, 100));
        points.Add(new Point(250, 200));
        points.Add(new Point(0, 300));
        points.Add(new Point(100, 200));
        points.Add(new Point(600, 700));
        points.Add(new Point(650, 700));
        points.Add(new Point(675, 700));
        points.Add(new Point(675, 710));
        points.Add(new Point(675, 720));
        points.Add(new Point(50, 400));
        points.Add(new Point(20, 200));
        points.Add(new Point(280, 80));
        points.Add(new Point(300, 100));
        points.Add(new Point(400, 100));
        points.Add(new Point(265, 85));
        points.Add(new Point(350, 150));

        //Khởi tại bán kính vùng lân cận
        double eps = 100.0;
		//Khởi tạo số lượng điểm tối thiểu
		int minPts = 3;
		List<List<Point>> clusters = TaoCum(points, eps, minPts);
		Console.Clear();
		//in tổng số điểm 
		Console.WriteLine("Co tat ca {0} diem :\n", points.Count);
		foreach (Point p in points) Console.Write(" {0} ", p);
		Console.WriteLine();
		//in ra từng cụm
		int tong = 0;
		int dem = 0;
		for (int i = 0; i < clusters.Count; i++)
		{
			int count = clusters[i].Count; //số điểm trong cum đó count
			//tìm các điểm tạo thành một cụm và in ra tổng số điểm của cụm đó 
			tong += count;  //đếm xem cụm có bao nhiêu điểm
			Console.WriteLine("\nCum {0} duoc tao ra tu {1} diem: \n", i + 1,count);
			dem++; //Tính dồn số cụm
			foreach (Point p in clusters[i]) Console.Write(" {0} ", p);
			Console.ReadKey();			
		}
		//in ra những điểm là điểm nhiễu (NOISE)
		tong = points.Count - tong; //cập nhật lại số điểm
		if (tong > 0)
		{
			Console.WriteLine("\nCo tat ca {0} diem nhieu! :\n", tong);
			foreach (Point p in points)
			{
				if (p.CumId == Point.NOISE) Console.Write(" {0} ", p);
			}
			Console.WriteLine();
		}
		else
		{
			Console.WriteLine("\nKhong co diem nhieu nao!");
		}
		Console.ReadKey();
		Console.WriteLine("Co tat ca {0} cum", dem);
	}
	//Tạo cụm bằng các điểm có  sẵn
	static List<List<Point>> TaoCum(List<Point> points, double eps, int minPts)
	{
		if (points == null) return null;
		List<List<Point>> cum = new List<List<Point>>();
		eps *= eps; // eps bình phương
		//Khởi tạo cụm ít nhất là 1
		int CumId = 1;
		for (int i = 0; i < points.Count; i++)
		{
			Point p = points[i];
			if (p.CumId == Point.chuaphanloai)
			{
				if (MoRongCum(points, p, CumId, eps, minPts))
                    CumId++;
			}
		}
		//phân loại các điểm thành cụm riêng của nó, nếu có
		int maxCumId = points.OrderBy(p => p.CumId).Last().CumId;
		if (maxCumId < 1) return cum; //không có cụm, trả về danh sách rỗng
		for (int i = 0; i < maxCumId; i++)
            cum.Add(new List<Point>());
		foreach (Point p in points)
		{
			if (p.CumId > 0)
                cum[p.CumId - 1].Add(p);    //
		}
		return cum;
	}
	//Khoảng cách từ lõi đến điểm gần nhất q <= eps(bán kính hình tròn vùng đó)
	static List<Point> TimDiemLanCan(List<Point> points, Point p, double eps)
	{
		List<Point> vunglancan = new List<Point>();
		for (int i = 0; i < points.Count; i++)
		{
			int kcbinh = Point.khoangcach(p, points[i]);
			if (kcbinh <= eps) vunglancan.Add(points[i]);
		}
		return vunglancan;
	}
	static bool MoRongCum(List<Point> points, Point p, int CumId, double eps, int minPts)
	{
		List<Point> hatgiong = TimDiemLanCan(points, p, eps); //hạt giống là điểm lõi
		if (hatgiong.Count < minPts) //không có điểm cốt lõi return false
		{
			p.CumId = Point.NOISE;
			return false;
		}
		else // các điểm trong vùng có thể truy cập trực tiếp từ 'p'
		{
			for (int i = 0; i < hatgiong.Count; i++)
                hatgiong[i].CumId = CumId;
			hatgiong.Remove(p); //loại bỏ sự xuất hiện đầu tiên của p ở đây là điểm lõi khỏi danh sách
			while (hatgiong.Count > 0)
			{
				Point hangxomP = hatgiong[0];
                //Tìm các thành phần được kết nối của các điểm cốt lõi trên đồ thị lân cận, bỏ qua tất cả các điểm không phải lõi.
                List<Point> ketqua = TimDiemLanCan(points, hangxomP, eps); 
				if (ketqua.Count >= minPts)
				{
					for (int i = 0; i < ketqua.Count; i++)
					{
						Point ketquaP = ketqua[i];
						if (ketquaP.CumId == Point.chuaphanloai || ketquaP.CumId == Point.NOISE)
						{
							if (ketquaP.CumId == Point.chuaphanloai) hatgiong.Add(ketquaP);
							ketquaP.CumId = CumId;
						}
					}
				}
				hatgiong.Remove(hangxomP);
			}
			return true;
		}
	}
}