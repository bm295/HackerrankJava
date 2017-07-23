import java.io.*;
import java.util.*;
public class Solution {

    public static void main(String[] args) {
        Scanner scan = new Scanner(System.in);
        String s = scan.nextLine();
        // Write your code here.
        String[] output = s.trim().split("[ !,?._'@]+");
        if (output.length == 1 && output[0].isEmpty()) {
            System.out.println(0);
        }
        else {
            System.out.println(output.length);
            for(String word : output) {
                System.out.println(word);
            }
        }        
        scan.close();
    }
}
